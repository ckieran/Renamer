using System.Text.Json;
using Renamer.Cli.Commands;
using Renamer.Core.Contracts;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;

namespace Renamer.Tests.CLI;

public sealed class CliPlanCommandTests : IDisposable
{
    private readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "renamer-cli-plan-tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void Handle_PlanWithValidArguments_WritesPlanJsonAndReturnsSuccess()
    {
        Directory.CreateDirectory(tempDirectory);
        var rootPath = Path.Combine(tempDirectory, "root");
        Directory.CreateDirectory(rootPath);
        var outputPath = Path.Combine(tempDirectory, "out", "rename-plan.json");
        var handler = new CliCommandHandler(new FakePlanBuilder(CreatePlan(rootPath)), new PlanSerializer());

        var result = handler.Handle(new CliCommand(
            CliCommandType.Plan,
            "plan",
            Arguments: ["--root", rootPath, "--out", outputPath]));

        Assert.Equal(ProcessExitCode.Success, result.ExitCode);
        Assert.True(File.Exists(outputPath));
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        Assert.Equal(rootPath, document.RootElement.GetProperty("rootPath").GetString());
    }

    [Fact]
    public void Handle_PlanWhenRootPathIsMissing_ReturnsIoFailure()
    {
        var handler = new CliCommandHandler(new FakePlanBuilder(CreatePlan("/unused")), new PlanSerializer());

        var result = handler.Handle(new CliCommand(
            CliCommandType.Plan,
            "plan",
            Arguments: ["--root", Path.Combine(tempDirectory, "missing"), "--out", Path.Combine(tempDirectory, "rename-plan.json")]));

        Assert.Equal(ProcessExitCode.IoFailure, result.ExitCode);
    }

    [Fact]
    public void Handle_PlanWhenPlanBuilderThrowsUnexpectedError_ReturnsUnexpectedRuntimeError()
    {
        Directory.CreateDirectory(tempDirectory);
        var rootPath = Path.Combine(tempDirectory, "root");
        Directory.CreateDirectory(rootPath);
        var handler = new CliCommandHandler(new ThrowingPlanBuilder(), new PlanSerializer());

        var result = handler.Handle(new CliCommand(
            CliCommandType.Plan,
            "plan",
            Arguments: ["--root", rootPath, "--out", Path.Combine(tempDirectory, "rename-plan.json")]));

        Assert.Equal(ProcessExitCode.UnexpectedRuntimeError, result.ExitCode);
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private static RenamePlan CreatePlan(string rootPath) =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            CreatedAtUtc = "2026-03-01T16:10:00Z",
            RootPath = rootPath,
            Operations =
            [
                new RenamePlanOperation
                {
                    OpId = "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    SourcePath = Path.Combine(rootPath, "Trip A"),
                    PlannedDestinationPath = Path.Combine(rootPath, "2024-06-12 - 2024-06-14 - Trip A"),
                    Reason = new RenamePlanReason
                    {
                        StartDate = "2024-06-12",
                        EndDate = "2024-06-14",
                        FilesConsidered = 12,
                        FilesSkippedMissingExif = 1
                    }
                }
            ],
            Summary = new RenamePlanSummary
            {
                OperationCount = 1,
                Warnings = 1
            }
        };

    private sealed class FakePlanBuilder(RenamePlan plan) : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => plan;
    }

    private sealed class ThrowingPlanBuilder : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => throw new InvalidOperationException("boom");
    }
}
