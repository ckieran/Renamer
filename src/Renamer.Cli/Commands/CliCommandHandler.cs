using System.Text.Json;
using Renamer.Cli.Resources;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;

namespace Renamer.Cli.Commands;

public sealed class CliCommandHandler : ICliCommandHandler
{
    private static readonly string[] HelpLines =
    [
        CliStrings.HelpHeader,
        CliStrings.HelpCommandsLabel,
        CliStrings.HelpCommandHelp,
        CliStrings.HelpCommandPlan,
        CliStrings.HelpCommandApply
    ];

    private readonly IPlanBuilder planBuilder;
    private readonly IPlanSerializer planSerializer;
    private readonly IApplyEngine applyEngine;
    private readonly IReportSerializer reportSerializer;

    public CliCommandHandler(
        IPlanBuilder planBuilder,
        IPlanSerializer planSerializer,
        IApplyEngine applyEngine,
        IReportSerializer reportSerializer)
    {
        this.planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
        this.planSerializer = planSerializer ?? throw new ArgumentNullException(nameof(planSerializer));
        this.applyEngine = applyEngine ?? throw new ArgumentNullException(nameof(applyEngine));
        this.reportSerializer = reportSerializer ?? throw new ArgumentNullException(nameof(reportSerializer));
    }

    public CommandResult Handle(CliCommand command)
    {
        return command.Type switch
        {
            CliCommandType.Help => new CommandResult(ProcessExitCode.Success, HelpLines),
            CliCommandType.Plan => HandlePlan(command),
            CliCommandType.Apply => HandleApply(command),
            CliCommandType.Invalid => new CommandResult(ProcessExitCode.ValidationFailure, HelpLines),
            _ => new CommandResult(ProcessExitCode.UnexpectedRuntimeError, [])
        };
    }

    private CommandResult HandlePlan(CliCommand command)
    {
        var arguments = command.Arguments ?? [];
        if (!TryGetOptionValue(arguments, "--root", out var rootPath) ||
            !TryGetOptionValue(arguments, "--out", out var outputPath))
        {
            return CreateValidationFailure(CliStrings.PlanErrorMissingArgs);
        }

        if (!Directory.Exists(rootPath))
        {
            return new CommandResult(ProcessExitCode.IoFailure, [string.Format(CliStrings.PlanErrorRootNotFound, rootPath)]);
        }

        if (IsDirectoryPath(outputPath))
        {
            return new CommandResult(ProcessExitCode.IoFailure, [string.Format(CliStrings.PlanErrorOutputIsDirectory, outputPath)]);
        }

        try
        {
            EnsureWritableOutputPath(outputPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [string.Format(CliStrings.PlanErrorOutputNotWritable, outputPath)]);
        }

        try
        {
            var plan = planBuilder.Build(rootPath);
            planSerializer.Write(outputPath, plan);
            return new CommandResult(ProcessExitCode.Success, []);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [CliStrings.PlanErrorBuildFileSystem]);
        }
        catch (Exception)
        {
            return new CommandResult(ProcessExitCode.UnexpectedRuntimeError, [CliStrings.PlanErrorBuildUnexpected]);
        }
    }

    private static bool TryGetOptionValue(IReadOnlyList<string> arguments, string optionName, out string value)
    {
        for (var index = 0; index < arguments.Count - 1; index++)
        {
            if (string.Equals(arguments[index], optionName, StringComparison.Ordinal))
            {
                value = arguments[index + 1];
                return !string.IsNullOrWhiteSpace(value);
            }
        }

        value = string.Empty;
        return false;
    }

    private CommandResult HandleApply(CliCommand command)
    {
        var arguments = command.Arguments ?? [];
        if (!TryGetOptionValue(arguments, "--plan", out var planPath) ||
            !TryGetOptionValue(arguments, "--out", out var outputPath))
        {
            return CreateValidationFailure(CliStrings.ApplyErrorMissingArgs);
        }

        if (!File.Exists(planPath))
        {
            return new CommandResult(ProcessExitCode.IoFailure, [string.Format(CliStrings.ApplyErrorPlanNotFound, planPath)]);
        }

        try
        {
            using var _ = File.Open(planPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [string.Format(CliStrings.ApplyErrorPlanNotReadable, planPath)]);
        }

        RenamePlan plan;
        try
        {
            plan = planSerializer.Read(planPath);
        }
        catch (Exception ex) when (ex is InvalidDataException or JsonException or NotSupportedException)
        {
            return new CommandResult(ProcessExitCode.InvalidOrUnsupportedPlanSchema, [CliStrings.ApplyErrorInvalidSchema]);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [CliStrings.ApplyErrorReadFileSystem]);
        }

        if (IsDirectoryPath(outputPath))
        {
            return new CommandResult(ProcessExitCode.IoFailure, [string.Format(CliStrings.ApplyErrorOutputIsDirectory, outputPath)]);
        }

        try
        {
            EnsureWritableOutputPath(outputPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [string.Format(CliStrings.ApplyErrorOutputNotWritable, outputPath)]);
        }

        try
        {
            var report = applyEngine.Execute(plan);
            reportSerializer.Write(outputPath, report);

            return string.Equals(report.Outcome, ApplyEngine.ConflictRetryLimitReachedOutcome, StringComparison.Ordinal)
                ? new CommandResult(ProcessExitCode.ConflictRetryLimitReached, [])
                : new CommandResult(ProcessExitCode.Success, []);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [CliStrings.ApplyErrorEngineFileSystem]);
        }
        catch (Exception)
        {
            return new CommandResult(ProcessExitCode.UnexpectedRuntimeError, [CliStrings.ApplyErrorEngineUnexpected]);
        }
    }

    private static void EnsureWritableOutputPath(string outputPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var fullOutputPath = Path.GetFullPath(outputPath);
        var directory = Path.GetDirectoryName(fullOutputPath) ?? Directory.GetCurrentDirectory();
        Directory.CreateDirectory(directory);

        var probePath = Path.Combine(directory, $".renamer-write-test-{Guid.NewGuid():N}.tmp");
        File.WriteAllText(probePath, string.Empty);
        File.Delete(probePath);
    }

    private static bool IsDirectoryPath(string outputPath)
    {
        var fullOutputPath = Path.GetFullPath(outputPath);
        return Directory.Exists(fullOutputPath) ||
               outputPath.EndsWith(Path.DirectorySeparatorChar) ||
               outputPath.EndsWith(Path.AltDirectorySeparatorChar);
    }

    private static CommandResult CreateValidationFailure(string message) =>
        new(ProcessExitCode.ValidationFailure, [message, .. HelpLines]);
}
