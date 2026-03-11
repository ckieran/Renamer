using System.Text.Json;
using Renamer.Core.Contracts;
using Renamer.Core.Serialization;

namespace Renamer.Tests.Core;

public sealed class ReportSerializerTests
{
    [Fact]
    public void Write_CreatesExpectedReportJsonShapeIncludingNullableFields()
    {
        var serializer = new ReportSerializer();
        var outputPath = Path.Combine(Path.GetTempPath(), $"rename-report-{Guid.NewGuid():N}.json");

        try
        {
            var report = CreateReport();

            serializer.Write(outputPath, report);

            var json = File.ReadAllText(outputPath);
            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;
            Assert.Equal("1.0", root.GetProperty("schemaVersion").GetString());
            Assert.Equal(report.PlanId, root.GetProperty("planId").GetString());
            Assert.Equal(report.StartedAtUtc, root.GetProperty("startedAtUtc").GetString());
            Assert.Equal(report.FinishedAtUtc, root.GetProperty("finishedAtUtc").GetString());

            var results = root.GetProperty("results");
            Assert.Equal(JsonValueKind.Array, results.ValueKind);
            Assert.Equal(3, results.GetArrayLength());

            var failedResult = results[1];
            Assert.Equal("failed", failedResult.GetProperty("status").GetString());
            Assert.Equal(JsonValueKind.Null, failedResult.GetProperty("actualDestinationPath").ValueKind);
            Assert.Equal(JsonValueKind.Null, failedResult.GetProperty("error").ValueKind);

            var summary = root.GetProperty("summary");
            Assert.Equal(1, summary.GetProperty("success").GetInt32());
            Assert.Equal(1, summary.GetProperty("failed").GetInt32());
            Assert.Equal(1, summary.GetProperty("skipped").GetInt32());
            Assert.Equal(1, summary.GetProperty("drifted").GetInt32());
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void Write_WhenOutputExists_OverwritesFile()
    {
        var serializer = new ReportSerializer();
        var outputPath = Path.Combine(Path.GetTempPath(), $"rename-report-{Guid.NewGuid():N}.json");

        try
        {
            File.WriteAllText(outputPath, "{\"stale\":true}");

            serializer.Write(outputPath, CreateReport());

            var json = File.ReadAllText(outputPath);
            Assert.DoesNotContain("stale", json);
            Assert.Contains("\"schemaVersion\": \"1.0\"", json);
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void Write_WhenSummaryInputIsWrong_RecomputesSummaryFromResults()
    {
        var serializer = new ReportSerializer();
        var outputPath = Path.Combine(Path.GetTempPath(), $"rename-report-{Guid.NewGuid():N}.json");
        var report = CreateReport();
        report = new RenameReport
        {
            SchemaVersion = report.SchemaVersion,
            PlanId = report.PlanId,
            StartedAtUtc = report.StartedAtUtc,
            FinishedAtUtc = report.FinishedAtUtc,
            Results = report.Results,
            Summary = new RenameReportSummary
            {
                Success = 99,
                Failed = 0,
                Skipped = 0,
                Drifted = 99
            }
        };

        try
        {
            serializer.Write(outputPath, report);

            using var document = JsonDocument.Parse(File.ReadAllText(outputPath));
            var summary = document.RootElement.GetProperty("summary");
            Assert.Equal(1, summary.GetProperty("success").GetInt32());
            Assert.Equal(1, summary.GetProperty("failed").GetInt32());
            Assert.Equal(1, summary.GetProperty("skipped").GetInt32());
            Assert.Equal(1, summary.GetProperty("drifted").GetInt32());
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    [Fact]
    public void Write_WhenAttemptsInvariantIsBroken_Throws()
    {
        var serializer = new ReportSerializer();
        var outputPath = Path.Combine(Path.GetTempPath(), $"rename-report-{Guid.NewGuid():N}.json");
        var report = CreateReport();
        report = new RenameReport
        {
            SchemaVersion = report.SchemaVersion,
            PlanId = report.PlanId,
            StartedAtUtc = report.StartedAtUtc,
            FinishedAtUtc = report.FinishedAtUtc,
            Results =
            [
                new RenameReportResult
                {
                    OpId = "broken",
                    SourcePath = "/photos/Broken",
                    PlannedDestinationPath = "/photos/Broken",
                    ActualDestinationPath = null,
                    Status = "failed",
                    Attempts = 0,
                    Warnings = [],
                    Error = "bad"
                }
            ],
            Summary = report.Summary
        };

        try
        {
            var exception = Assert.Throws<InvalidOperationException>(() => serializer.Write(outputPath, report));
            Assert.Contains("result.attempts", exception.Message);
            Assert.False(File.Exists(outputPath));
        }
        finally
        {
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }
        }
    }

    private static RenameReport CreateReport() =>
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
                    OpId = "success-1",
                    SourcePath = "/photos/Trip A",
                    PlannedDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
                    ActualDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A (1)",
                    Status = "success",
                    Attempts = 2,
                    Warnings = ["Destination conflict; applied suffix (1)."],
                    Error = null
                },
                new RenameReportResult
                {
                    OpId = "failed-1",
                    SourcePath = "/photos/Trip B",
                    PlannedDestinationPath = "/photos/2024-06-15 - Trip B",
                    ActualDestinationPath = null,
                    Status = "failed",
                    Attempts = 1,
                    Warnings = [],
                    Error = null
                },
                new RenameReportResult
                {
                    OpId = "skipped-1",
                    SourcePath = "/photos/Trip C",
                    PlannedDestinationPath = "/photos/2024-06-16 - Trip C",
                    ActualDestinationPath = null,
                    Status = "skipped",
                    Attempts = 1,
                    Warnings = ["Skipped for test coverage."],
                    Error = "Not applied."
                }
            ],
            Summary = new RenameReportSummary
            {
                Success = 0,
                Failed = 0,
                Skipped = 0,
                Drifted = 0
            }
        };
}
