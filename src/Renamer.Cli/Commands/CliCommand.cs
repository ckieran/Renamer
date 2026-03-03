namespace Renamer.Cli.Commands;

public sealed record CliCommand(
    CliCommandType Type,
    string? CommandText,
    CliCommandParseError ParseError = CliCommandParseError.None);
