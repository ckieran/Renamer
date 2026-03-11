using Renamer.Cli.Commands;
using Microsoft.Extensions.Logging;

namespace Renamer.Cli;

public sealed class CliCommandDispatcher(
    TextWriter output,
    ICliCommandHandler handler,
    ILogger<CliCommandDispatcher> logger)
{
    public int Dispatch(string[] args)
    {
        var parsedCommand = CliCommandParser.Parse(args);
        switch (parsedCommand.Type)
        {
            case CliCommandType.Invalid when parsedCommand.ParseError == CliCommandParseError.MissingCommand:
                logger.LogWarning("No command provided.");
                break;
            case CliCommandType.Invalid:
                logger.LogWarning("Unsupported CLI command {Command}.", parsedCommand.CommandText);
                break;
            case CliCommandType.Plan:
                logger.LogInformation("Accepted CLI command {Command}.", parsedCommand.CommandText);
                break;
            case CliCommandType.Apply:
                logger.LogInformation("Accepted CLI command {Command}. Command implementation is pending.", parsedCommand.CommandText);
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
