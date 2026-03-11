using System.Text.Json;
using Renamer.Core.Contracts;

namespace Renamer.Tests.Core;

public sealed class CoreContractModelsTests
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = false
    };

    [Fact]
    public void RenamePlan_RoundTripSerialization_UsesSchemaShape()
    {
        var plan = new RenamePlan
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
                        FilesConsidered = 120,
                        FilesSkippedMissingExif = 3
                    }
                }
            ],
            Summary = new RenamePlanSummary
            {
                OperationCount = 1,
                Warnings = 3
            }
        };

        var json = JsonSerializer.Serialize(plan, SerializerOptions);
        var roundTripped = JsonSerializer.Deserialize<RenamePlan>(json, SerializerOptions);

        Assert.Contains("\"schemaVersion\":\"1.0\"", json);
        Assert.Contains("\"createdAtUtc\":\"2026-03-01T16:10:00Z\"", json);
        Assert.Contains("\"filesSkippedMissingExif\":3", json);
        Assert.NotNull(roundTripped);
        Assert.Equal(plan.PlanId, roundTripped.PlanId);
        Assert.Equal(plan.Operations[0].PlannedDestinationPath, roundTripped.Operations[0].PlannedDestinationPath);
        Assert.Equal(plan.Summary.OperationCount, roundTripped.Summary.OperationCount);
    }

    [Fact]
    public void RenameReport_RoundTripSerialization_UsesSchemaShapeAndNullableFields()
    {
        var report = new RenameReport
        {
            Outcome = "conflictRetryLimitReached",
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
                    Attempts = 1,
                    Warnings = ["example warning"],
                    Error = null
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

        var json = JsonSerializer.Serialize(report, SerializerOptions);
        var roundTripped = JsonSerializer.Deserialize<RenameReport>(json, SerializerOptions);

        Assert.Contains("\"actualDestinationPath\":null", json);
        Assert.Contains("\"error\":null", json);
        Assert.Contains("\"outcome\":\"conflictRetryLimitReached\"", json);
        Assert.Contains("\"status\":\"failed\"", json);
        Assert.NotNull(roundTripped);
        Assert.Equal(report.Outcome, roundTripped.Outcome);
        Assert.Null(roundTripped.Results[0].ActualDestinationPath);
        Assert.Null(roundTripped.Results[0].Error);
        Assert.Equal(report.Summary.Failed, roundTripped.Summary.Failed);
    }
}
