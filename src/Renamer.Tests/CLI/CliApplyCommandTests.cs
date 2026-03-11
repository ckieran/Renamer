using System.Text.Json;
using Renamer.Cli.Commands;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;

namespace Renamer.Tests.CLI;

public sealed class CliApplyCommandTests : IDisposable
{
    private readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "renamer-cli-apply-tests", Guid.NewGuid().ToString("N"));

    [Fact]
    public void Handle_ApplyWithValidArguments_WritesReportJsonAndReturnsSuccess()
    {
        Directory.CreateDirectory(tempDirectory);
        var planPath = WritePlanFile(CreatePlan());
        var outputPath = Path.Combine(tempDirectory, "out", "rename-report.json");
        var handler = new CliCommandHandler(
            new StubPlanBuilder(),
            new PlanSerializer(),
            new FakeApplyEngine(CreateSuccessReport()),
            new ReportSerializer());

        var result = handler.Handle(new CliCommand(
            CliCommandType.Apply,
            "apply",
            Arguments: ["--plan", planPath, "--out", outputPath]));

        Assert.Equal(ProcessExitCode.Success, result.ExitCode);
        Assert.True(File.Exists(outputPath));
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        Assert.Equal("1.0", document.RootElement.GetProperty("schemaVersion").GetString());
        Assert.Equal(1, document.RootElement.GetProperty("summary").GetProperty("success").GetInt32());
    }

    [Fact]
    public void Handle_ApplyWhenSchemaVersionIsUnsupported_ReturnsInvalidSchemaCode()
    {
        Directory.CreateDirectory(tempDirectory);
        var planPath = Path.Combine(tempDirectory, "rename-plan.json");
        File.WriteAllText(planPath, """
            {
              "schemaVersion": "2.0",
              "planId": "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
              "createdAtUtc": "2026-03-01T16:10:00Z",
              "rootPath": "/photos",
              "operations": [],
              "summary": {
                "operationCount": 0,
                "warnings": 0
              }
            }
            """);
        var handler = new CliCommandHandler(
            new StubPlanBuilder(),
            new PlanSerializer(),
            new FakeApplyEngine(CreateSuccessReport()),
            new ReportSerializer());

        var result = handler.Handle(new CliCommand(
            CliCommandType.Apply,
            "apply",
            Arguments: ["--plan", planPath, "--out", Path.Combine(tempDirectory, "rename-report.json")]));

        Assert.Equal(ProcessExitCode.InvalidOrUnsupportedPlanSchema, result.ExitCode);
    }

    [Fact]
    public void Handle_ApplyWhenRetryLimitIsReached_WritesFailedReportAndReturnsConflictCode()
    {
        Directory.CreateDirectory(tempDirectory);
        var planPath = WritePlanFile(CreatePlan());
        var outputPath = Path.Combine(tempDirectory, "out", "rename-report.json");
        var handler = new CliCommandHandler(
            new StubPlanBuilder(),
            new PlanSerializer(),
            new FakeApplyEngine(CreateRetryLimitFailureReport()),
            new ReportSerializer());

        var result = handler.Handle(new CliCommand(
            CliCommandType.Apply,
            "apply",
            Arguments: ["--plan", planPath, "--out", outputPath]));

        Assert.Equal(ProcessExitCode.ConflictRetryLimitReached, result.ExitCode);
        Assert.True(File.Exists(outputPath));
        using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
        Assert.Equal(1, document.RootElement.GetProperty("summary").GetProperty("failed").GetInt32());
        Assert.Equal("failed", document.RootElement.GetProperty("results")[0].GetProperty("status").GetString());
    }

    public void Dispose()
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(tempDirectory, recursive: true);
        }
    }

    private string WritePlanFile(RenamePlan plan)
    {
        var planPath = Path.Combine(tempDirectory, "input", "rename-plan.json");
        Directory.CreateDirectory(Path.GetDirectoryName(planPath)!);
        new PlanSerializer().Write(planPath, plan);
        return planPath;
    }

    private static RenamePlan CreatePlan() =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            CreatedAtUtc = "2026-03-01T16:10:00Z",
            RootPath = "/photos",
            Operations =
            [
                new RenamePlanOperation
                {
                    OpId = "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    SourcePath = "/photos/Trip A",
                    PlannedDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
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

    private static RenameReport CreateSuccessReport() =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            StartedAtUtc = "2026-03-01T16:11:00Z",
            FinishedAtUtc = "2026-03-01T16:11:01Z",
            Results =
            [
                new RenameReportResult
                {
                    OpId = "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    SourcePath = "/photos/Trip A",
                    PlannedDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
                    ActualDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
                    Status = "success",
                    Attempts = 1,
                    Warnings = [],
                    Error = null
                }
            ],
            Summary = new RenameReportSummary
            {
                Success = 1,
                Failed = 0,
                Skipped = 0,
                Drifted = 0
            }
        };

    private static RenameReport CreateRetryLimitFailureReport() =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            StartedAtUtc = "2026-03-01T16:11:00Z",
            FinishedAtUtc = "2026-03-01T16:11:01Z",
            Results =
            [
                new RenameReportResult
                {
                    OpId = "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    SourcePath = "/photos/Trip A",
                    PlannedDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
                    ActualDestinationPath = null,
                    Status = "failed",
                    Attempts = 11,
                    Warnings = [],
                    Error = "Destination conflict unresolved after 10 suffix retries."
                }
            ],
            Summary = new RenameReportSummary
            {
                Success = 0,
                Failed = 1,
                Skipped = 0,
                Drifted = 0
            }
        };

    private sealed class StubPlanBuilder : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => throw new NotImplementedException();
    }

    private sealed class FakeApplyEngine(RenameReport report) : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => report;
    }
}
