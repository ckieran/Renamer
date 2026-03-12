using Renamer.Cli.Commands;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
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
        Assert.Contains(result.OutputLines, line => line.Contains("Missing required arguments for 'plan'", StringComparison.Ordinal));
        Assert.Contains("Available commands:", result.OutputLines);
    }

    [Fact]
    public void Handle_ApplyWithMissingArguments_ReturnsValidationFailure()
    {
        var result = CreateHandler().Handle(new CliCommand(CliCommandType.Apply, "apply", Arguments: ["--plan", "/tmp/plan.json"]));

        Assert.Equal(ProcessExitCode.ValidationFailure, result.ExitCode);
        Assert.Contains(result.OutputLines, line => line.Contains("Missing required arguments for 'apply'", StringComparison.Ordinal));
        Assert.Contains("Available commands:", result.OutputLines);
    }

    private static CliCommandHandler CreateHandler() =>
        new(new StubPlanBuilder(), new StubPlanSerializer(), new StubApplyEngine(), new StubReportSerializer());

    private sealed class StubPlanBuilder : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => throw new NotImplementedException();
    }

    private sealed class StubPlanSerializer : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => throw new NotImplementedException();

        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class StubApplyEngine : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class StubReportSerializer : IReportSerializer
    {
        public void Write(string outputPath, RenameReport report) => throw new NotImplementedException();
    }
}
