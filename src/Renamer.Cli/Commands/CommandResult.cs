namespace Renamer.Cli.Commands;

public sealed record CommandResult(ProcessExitCode ExitCode, IReadOnlyList<string> OutputLines);
