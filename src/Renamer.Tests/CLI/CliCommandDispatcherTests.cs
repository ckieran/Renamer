using Renamer.Cli;

namespace Renamer.Tests.CLI;

public sealed class CliCommandDispatcherTests
{
    [Fact]
    public void Dispatch_HelpCommand_PrintsAvailableCommandsAndReturnsSuccess()
    {
        using var writer = new StringWriter();

        var exitCode = CliCommandDispatcher.Dispatch(["help"], writer);
        var text = writer.ToString();

        Assert.Equal(0, exitCode);
        Assert.Contains("Available commands:", text);
        Assert.Contains("plan --root <path> --out <path>", text);
        Assert.Contains("apply --plan <path> --out <path>", text);
    }

    [Fact]
    public void Dispatch_UnsupportedCommand_PrintsHelpAndReturnsValidationCode()
    {
        using var writer = new StringWriter();

        var exitCode = CliCommandDispatcher.Dispatch(["wat"], writer);
        var text = writer.ToString();

        Assert.Equal(2, exitCode);
        Assert.Contains("Available commands:", text);
    }
}
