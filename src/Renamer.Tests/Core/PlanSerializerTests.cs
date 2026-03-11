using System.Text.Json;
using Renamer.Core.Contracts;
using Renamer.Core.Serialization;

namespace Renamer.Tests.Core;

public sealed class PlanSerializerTests
{
    [Fact]
    public void Write_CreatesExpectedPlanJsonShape()
    {
        var serializer = new PlanSerializer();
        var outputPath = Path.Combine(Path.GetTempPath(), $"rename-plan-{Guid.NewGuid():N}.json");

        try
        {
            var plan = CreatePlan();

            serializer.Write(outputPath, plan);

            var json = File.ReadAllText(outputPath);
            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;
            Assert.Equal("1.0", root.GetProperty("schemaVersion").GetString());
            Assert.Equal(plan.PlanId, root.GetProperty("planId").GetString());
            Assert.Equal(plan.CreatedAtUtc, root.GetProperty("createdAtUtc").GetString());
            Assert.Equal(plan.RootPath, root.GetProperty("rootPath").GetString());

            var operations = root.GetProperty("operations");
            Assert.Equal(JsonValueKind.Array, operations.ValueKind);
            Assert.Single(operations.EnumerateArray());

            var operation = operations[0];
            Assert.Equal(plan.Operations[0].OpId, operation.GetProperty("opId").GetString());
            Assert.Equal(plan.Operations[0].SourcePath, operation.GetProperty("sourcePath").GetString());
            Assert.Equal(plan.Operations[0].PlannedDestinationPath, operation.GetProperty("plannedDestinationPath").GetString());

            var reason = operation.GetProperty("reason");
            Assert.Equal(plan.Operations[0].Reason.StartDate, reason.GetProperty("startDate").GetString());
            Assert.Equal(plan.Operations[0].Reason.EndDate, reason.GetProperty("endDate").GetString());
            Assert.Equal(plan.Operations[0].Reason.FilesConsidered, reason.GetProperty("filesConsidered").GetInt32());
            Assert.Equal(plan.Operations[0].Reason.FilesSkippedMissingExif, reason.GetProperty("filesSkippedMissingExif").GetInt32());

            var summary = root.GetProperty("summary");
            Assert.Equal(plan.Summary.OperationCount, summary.GetProperty("operationCount").GetInt32());
            Assert.Equal(plan.Summary.Warnings, summary.GetProperty("warnings").GetInt32());

            Assert.Equal(
                summary.GetProperty("operationCount").GetInt32(),
                operations.GetArrayLength());
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
        var serializer = new PlanSerializer();
        var outputPath = Path.Combine(Path.GetTempPath(), $"rename-plan-{Guid.NewGuid():N}.json");

        try
        {
            File.WriteAllText(outputPath, "{\"stale\":true}");
            var plan = CreatePlan();

            serializer.Write(outputPath, plan);

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
    public void Write_WhenOperationCountInvariantIsBroken_Throws()
    {
        var serializer = new PlanSerializer();
        var outputPath = Path.Combine(Path.GetTempPath(), $"rename-plan-{Guid.NewGuid():N}.json");
        var plan = CreatePlan();
        plan = new RenamePlan
        {
            SchemaVersion = plan.SchemaVersion,
            PlanId = plan.PlanId,
            CreatedAtUtc = plan.CreatedAtUtc,
            RootPath = plan.RootPath,
            Operations = plan.Operations,
            Summary = new RenamePlanSummary
            {
                OperationCount = 2,
                Warnings = plan.Summary.Warnings
            }
        };

        try
        {
            var exception = Assert.Throws<InvalidOperationException>(() => serializer.Write(outputPath, plan));
            Assert.Contains("summary.operationCount", exception.Message);
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

    private static RenamePlan CreatePlan()
    {
        return new RenamePlan
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
    }
}
