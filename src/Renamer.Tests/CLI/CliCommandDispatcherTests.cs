using Renamer.Cli;
using Renamer.Cli.Commands;
using Microsoft.Extensions.Logging.Abstractions;

namespace Renamer.Tests.CLI;

public sealed class CliCommandDispatcherTests
{
    [Fact]
    public void Dispatch_HelpCommand_PrintsAvailableCommandsAndReturnsSuccess()
    {
        using var writer = new StringWriter();
        var dispatcher = new CliCommandDispatcher(writer, new CliCommandHandler(), NullLogger<CliCommandDispatcher>.Instance);

        var exitCode = dispatcher.Dispatch(["help"]);
        var text = writer.ToString();

        Assert.Equal((int)ProcessExitCode.Success, exitCode);
        Assert.Contains("Available commands:", text);
        Assert.Contains("plan --root <path> --out <path>", text);
        Assert.Contains("apply --plan <path> --out <path>", text);
    }

    [Fact]
    public void Dispatch_UnsupportedCommand_PrintsHelpAndReturnsValidationCode()
    {
        using var writer = new StringWriter();
        var dispatcher = new CliCommandDispatcher(writer, new CliCommandHandler(), NullLogger<CliCommandDispatcher>.Instance);

        var exitCode = dispatcher.Dispatch(["wat"]);
        var text = writer.ToString();

        Assert.Equal((int)ProcessExitCode.ValidationFailure, exitCode);
        Assert.Contains("Available commands:", text);
    }
}
