using Renamer.Cli.Commands;

namespace Renamer.Tests.CLI;

public sealed class CliCommandHandlerTests
{
    private readonly CliCommandHandler _handler = new();

    [Fact]
    public void Handle_Help_ReturnsSuccessAndHelpLines()
    {
        var result = _handler.Handle(new CliCommand(CliCommandType.Help, "help"));

        Assert.Equal(ProcessExitCode.Success, result.ExitCode);
        Assert.Contains("Available commands:", result.OutputLines);
    }

    [Fact]
    public void Handle_Invalid_ReturnsValidationFailureAndHelpLines()
    {
        var result = _handler.Handle(
            new CliCommand(CliCommandType.Invalid, "wat", CliCommandParseError.UnsupportedCommand));

        Assert.Equal(ProcessExitCode.ValidationFailure, result.ExitCode);
        Assert.Contains("Available commands:", result.OutputLines);
    }
}
