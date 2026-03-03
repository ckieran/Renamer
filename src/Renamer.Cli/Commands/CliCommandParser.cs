namespace Renamer.Cli.Commands;

public static class CliCommandParser
{
    public static CliCommand Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return new CliCommand(CliCommandType.Invalid, null, CliCommandParseError.MissingCommand);
        }

        var command = args[0].Trim().ToLowerInvariant();
        return command switch
        {
            "help" or "--help" or "-h" => new CliCommand(CliCommandType.Help, command),
            "plan" => new CliCommand(CliCommandType.Plan, command),
            "apply" => new CliCommand(CliCommandType.Apply, command),
            _ => new CliCommand(CliCommandType.Invalid, command, CliCommandParseError.UnsupportedCommand)
        };
    }
}
