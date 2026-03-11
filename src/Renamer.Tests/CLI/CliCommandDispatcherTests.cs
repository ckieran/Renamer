using Renamer.Cli;
using Renamer.Cli.Commands;
using Renamer.Core.Contracts;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Microsoft.Extensions.Logging.Abstractions;

namespace Renamer.Tests.CLI;

public sealed class CliCommandDispatcherTests
{
    [Fact]
    public void Dispatch_HelpCommand_PrintsAvailableCommandsAndReturnsSuccess()
    {
        using var writer = new StringWriter();
        var dispatcher = new CliCommandDispatcher(writer, CreateHandler(), NullLogger<CliCommandDispatcher>.Instance);

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
        var dispatcher = new CliCommandDispatcher(writer, CreateHandler(), NullLogger<CliCommandDispatcher>.Instance);

        var exitCode = dispatcher.Dispatch(["wat"]);
        var text = writer.ToString();

        Assert.Equal((int)ProcessExitCode.ValidationFailure, exitCode);
        Assert.Contains("Available commands:", text);
    }

    private static CliCommandHandler CreateHandler() =>
        new(new StubPlanBuilder(), new StubPlanSerializer());

    private sealed class StubPlanBuilder : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => throw new NotImplementedException();
    }

    private sealed class StubPlanSerializer : IPlanSerializer
    {
        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }
}
