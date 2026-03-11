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

    public CliCommandHandler(IPlanBuilder planBuilder, IPlanSerializer planSerializer)
    {
        this.planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
        this.planSerializer = planSerializer ?? throw new ArgumentNullException(nameof(planSerializer));
    }

    public CommandResult Handle(CliCommand command)
    {
        return command.Type switch
        {
            CliCommandType.Help => new CommandResult(ProcessExitCode.Success, HelpLines),
            CliCommandType.Plan => HandlePlan(command),
            CliCommandType.Apply => new CommandResult(ProcessExitCode.Success, []),
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
