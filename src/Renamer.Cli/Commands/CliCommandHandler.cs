using System.Text.Json;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;

namespace Renamer.Cli.Commands;

public sealed class CliCommandHandler : ICliCommandHandler
{
    private static readonly string[] HelpLines =
    [
        "Renamer CLI",
        "Available commands:",
        "  help                Show this help.",
        "  plan --root <path> --out <path>",
        "  apply --plan <path> --out <path>"
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
            return new CommandResult(ProcessExitCode.ValidationFailure, HelpLines);
        }

        if (!Directory.Exists(rootPath))
        {
            return new CommandResult(ProcessExitCode.IoFailure, [$"Root path '{rootPath}' does not exist or is not a directory."]);
        }

        try
        {
            EnsureWritableOutputPath(outputPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [$"Output path '{outputPath}' is not writable: {ex.Message}"]);
        }

        try
        {
            var plan = planBuilder.Build(rootPath);
            planSerializer.Write(outputPath, plan);
            return new CommandResult(ProcessExitCode.Success, []);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [ex.Message]);
        }
        catch (Exception ex)
        {
            return new CommandResult(ProcessExitCode.UnexpectedRuntimeError, [ex.Message]);
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
            return new CommandResult(ProcessExitCode.ValidationFailure, HelpLines);
        }

        if (!File.Exists(planPath))
        {
            return new CommandResult(ProcessExitCode.IoFailure, [$"Plan path '{planPath}' does not exist."]);
        }

        try
        {
            using var _ = File.Open(planPath, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [$"Plan path '{planPath}' is not readable: {ex.Message}"]);
        }

        RenamePlan plan;
        try
        {
            plan = planSerializer.Read(planPath);
        }
        catch (Exception ex) when (ex is InvalidDataException or JsonException or NotSupportedException)
        {
            return new CommandResult(ProcessExitCode.InvalidOrUnsupportedPlanSchema, [ex.Message]);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [ex.Message]);
        }

        try
        {
            EnsureWritableOutputPath(outputPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [$"Output path '{outputPath}' is not writable: {ex.Message}"]);
        }

        try
        {
            var report = applyEngine.Execute(plan);
            reportSerializer.Write(outputPath, report);

            return IsConflictRetryLimitFailure(report)
                ? new CommandResult(ProcessExitCode.ConflictRetryLimitReached, [])
                : new CommandResult(ProcessExitCode.Success, []);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new CommandResult(ProcessExitCode.IoFailure, [ex.Message]);
        }
        catch (Exception ex)
        {
            return new CommandResult(ProcessExitCode.UnexpectedRuntimeError, [ex.Message]);
        }
    }

    private static bool IsConflictRetryLimitFailure(RenameReport report) =>
        report.Results.Any(result =>
            string.Equals(result.Status, "failed", StringComparison.Ordinal) &&
            result.Error is not null &&
            result.Error.Contains("suffix retries", StringComparison.Ordinal));

    private static void EnsureWritableOutputPath(string outputPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        var fullOutputPath = Path.GetFullPath(outputPath);
        var directory = Path.GetDirectoryName(fullOutputPath);
        if (string.IsNullOrWhiteSpace(directory))
        {
            directory = Directory.GetCurrentDirectory();
        }

        Directory.CreateDirectory(directory);

        var probePath = Path.Combine(directory, $".renamer-write-test-{Guid.NewGuid():N}.tmp");
        File.WriteAllText(probePath, string.Empty);
        File.Delete(probePath);
    }
}
