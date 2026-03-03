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

    public CommandResult Handle(CliCommand command)
    {
        return command.Type switch
        {
            CliCommandType.Help => new CommandResult(ProcessExitCode.Success, HelpLines),
            CliCommandType.Plan => new CommandResult(ProcessExitCode.Success, []),
            CliCommandType.Apply => new CommandResult(ProcessExitCode.Success, []),
            CliCommandType.Invalid => new CommandResult(ProcessExitCode.ValidationFailure, HelpLines),
            _ => new CommandResult(ProcessExitCode.UnexpectedRuntimeError, [])
        };
    }
}
