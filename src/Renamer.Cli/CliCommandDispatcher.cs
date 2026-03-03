using Renamer.Cli.Commands;
using Serilog;

namespace Renamer.Cli;

public static class CliCommandDispatcher
{
    public static int Dispatch(string[] args, TextWriter? output = null, ICliCommandHandler? handler = null)
    {
        output ??= Console.Out;
        handler ??= new CliCommandHandler();

        var parsedCommand = CliCommandParser.Parse(args);
        switch (parsedCommand.Type)
        {
            case CliCommandType.Invalid when parsedCommand.ParseError == CliCommandParseError.MissingCommand:
                Log.Warning("No command provided.");
                break;
            case CliCommandType.Invalid:
                Log.Warning("Unsupported CLI command {Command}.", parsedCommand.CommandText);
                break;
            case CliCommandType.Plan:
            case CliCommandType.Apply:
                Log.Information("Accepted CLI command {Command}. Command implementation is pending.", parsedCommand.CommandText);
                break;
        }

        var result = handler.Handle(parsedCommand);
        foreach (var line in result.OutputLines)
        {
            output.WriteLine(line);
        }

        return (int)result.ExitCode;
    }
}
