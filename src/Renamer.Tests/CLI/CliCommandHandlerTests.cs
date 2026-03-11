using Renamer.Cli.Commands;
using Renamer.Core.Contracts;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;

namespace Renamer.Tests.CLI;

public sealed class CliCommandHandlerTests
{
    [Fact]
    public void Handle_Help_ReturnsSuccessAndHelpLines()
    {
        var result = CreateHandler().Handle(new CliCommand(CliCommandType.Help, "help"));

        Assert.Equal(ProcessExitCode.Success, result.ExitCode);
        Assert.Contains("Available commands:", result.OutputLines);
    }

    [Fact]
    public void Handle_Invalid_ReturnsValidationFailureAndHelpLines()
    {
        var result = CreateHandler().Handle(
            new CliCommand(CliCommandType.Invalid, "wat", CliCommandParseError.UnsupportedCommand));

        Assert.Equal(ProcessExitCode.ValidationFailure, result.ExitCode);
        Assert.Contains("Available commands:", result.OutputLines);
    }

    [Fact]
    public void Handle_PlanWithMissingArguments_ReturnsValidationFailure()
    {
        var result = CreateHandler().Handle(new CliCommand(CliCommandType.Plan, "plan", Arguments: ["--root", "/tmp/root"]));

        Assert.Equal(ProcessExitCode.ValidationFailure, result.ExitCode);
        Assert.Contains("Available commands:", result.OutputLines);
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
