namespace Renamer.Cli.Commands;

public static class CliCommandParser
{
    public static CliCommand Parse(string[] args)
    {
        if (args.Length == 0)
            return new CliCommand(CliCommandType.Invalid, null, CliCommandParseError.MissingCommand);

        var command = args[0].Trim().ToLowerInvariant();
        var commandArguments = args.Skip(1).ToArray();
        return command switch
        {
            "help" or "--help" or "-h" => new CliCommand(CliCommandType.Help, command, Arguments: commandArguments),
            "plan" => new CliCommand(CliCommandType.Plan, command, Arguments: commandArguments),
            "apply" => new CliCommand(CliCommandType.Apply, command, Arguments: commandArguments),
            _ => new CliCommand(CliCommandType.Invalid, command, CliCommandParseError.UnsupportedCommand, commandArguments)
        };
    }
}
