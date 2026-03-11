using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.IO;
using Renamer.Core.Time;

namespace Renamer.Tests.Core;

public sealed class ApplyEngineTests
{
    [Fact]
    public void Execute_WhenPlannedDestinationIsAvailable_RenamesSuccessfullyOnFirstAttempt()
    {
        var directoryMover = new FakeDirectoryMover();
        var clock = new FakeClock(
            new DateTimeOffset(2026, 3, 11, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 11, 10, 0, 1, TimeSpan.Zero));
        var sut = new ApplyEngine(new ConflictRetryPolicy(), directoryMover, clock);
        var plan = CreatePlan();

        var report = sut.Execute(plan);

        var result = Assert.Single(report.Results);
        Assert.Equal(ApplyEngine.CompletedOutcome, report.Outcome);
        Assert.Equal("success", result.Status);
        Assert.Equal(1, result.Attempts);
        Assert.Equal(plan.Operations[0].PlannedDestinationPath, result.ActualDestinationPath);
        Assert.Empty(result.Warnings);
        Assert.Null(result.Error);
        Assert.Equal(plan.Operations[0].SourcePath, directoryMover.MoveCalls.Single().SourcePath);
        Assert.Equal(plan.Operations[0].PlannedDestinationPath, directoryMover.MoveCalls.Single().DestinationPath);
        Assert.Equal(1, report.Summary.Success);
        Assert.Equal(0, report.Summary.Failed);
        Assert.Equal(0, report.Summary.Drifted);
        Assert.Equal("2026-03-11T10:00:00Z", report.StartedAtUtc);
        Assert.Equal("2026-03-11T10:00:01Z", report.FinishedAtUtc);
    }

    [Fact]
    public void Execute_WhenConflictsExist_RetriesUntilFirstAvailableSuffix()
    {
        var directoryMover = new FakeDirectoryMover(
            existingPaths:
            [
                "/photos/2024-06-12 - 2024-06-14 - Trip A",
                "/photos/2024-06-12 - 2024-06-14 - Trip A (1)"
            ]);
        var clock = new FakeClock(
            new DateTimeOffset(2026, 3, 11, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 11, 10, 0, 1, TimeSpan.Zero));
        var sut = new ApplyEngine(new ConflictRetryPolicy(), directoryMover, clock);
        var plan = CreatePlan();

        var report = sut.Execute(plan);

        var result = Assert.Single(report.Results);
        Assert.Equal(ApplyEngine.CompletedOutcome, report.Outcome);
        Assert.Equal("success", result.Status);
        Assert.Equal(3, result.Attempts);
        Assert.Equal("/photos/2024-06-12 - 2024-06-14 - Trip A (2)", result.ActualDestinationPath);
        Assert.Equal(["Destination conflict; applied suffix (2)."], result.Warnings);
        Assert.Equal(1, report.Summary.Success);
        Assert.Equal(1, report.Summary.Drifted);
    }

    [Fact]
    public void Execute_WhenRetryLimitIsExceeded_FailsCurrentOperationAndAbortsPlan()
    {
        var existingPaths = new List<string> { "/photos/2024-06-12 - 2024-06-14 - Trip A" };
        for (var suffix = 1; suffix <= 10; suffix++)
        {
            existingPaths.Add($"/photos/2024-06-12 - 2024-06-14 - Trip A ({suffix})");
        }

        var directoryMover = new FakeDirectoryMover(existingPaths);
        var clock = new FakeClock(
            new DateTimeOffset(2026, 3, 11, 10, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 3, 11, 10, 0, 1, TimeSpan.Zero));
        var sut = new ApplyEngine(new ConflictRetryPolicy(), directoryMover, clock);
        var plan = CreatePlanWithTwoOperations();

        var report = sut.Execute(plan);

        var result = Assert.Single(report.Results);
        Assert.Equal(ApplyEngine.ConflictRetryLimitReachedOutcome, report.Outcome);
        Assert.Equal("failed", result.Status);
        Assert.Equal(11, result.Attempts);
        Assert.Null(result.ActualDestinationPath);
        Assert.Empty(result.Warnings);
        Assert.Contains("10 suffix retries", result.Error);
        Assert.Empty(directoryMover.MoveCalls);
        Assert.Equal(0, report.Summary.Success);
        Assert.Equal(1, report.Summary.Failed);
        Assert.Equal(0, report.Summary.Drifted);
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
                CreateOperation(
                    "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    "/photos/Trip A",
                    "/photos/2024-06-12 - 2024-06-14 - Trip A")
            ],
            Summary = new RenamePlanSummary
            {
                OperationCount = 1,
                Warnings = 0
            }
        };

    private static RenamePlan CreatePlanWithTwoOperations() =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            CreatedAtUtc = "2026-03-01T16:10:00Z",
            RootPath = "/photos",
            Operations =
            [
                CreateOperation(
                    "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    "/photos/Trip A",
                    "/photos/2024-06-12 - 2024-06-14 - Trip A"),
                CreateOperation(
                    "0e46e38f-9801-42ac-a2cf-8d184fa6a5f9",
                    "/photos/Trip B",
                    "/photos/2024-06-15 - Trip B")
            ],
            Summary = new RenamePlanSummary
            {
                OperationCount = 2,
                Warnings = 0
            }
        };

    private static RenamePlanOperation CreateOperation(string opId, string sourcePath, string plannedDestinationPath) =>
        new()
        {
            OpId = opId,
            SourcePath = sourcePath,
            PlannedDestinationPath = plannedDestinationPath,
            Reason = new RenamePlanReason
            {
                StartDate = "2024-06-12",
                EndDate = "2024-06-14",
                FilesConsidered = 12,
                FilesSkippedMissingExif = 0
            }
        };

    private sealed class FakeDirectoryMover(IEnumerable<string>? existingPaths = null) : IDirectoryMover
    {
        private readonly HashSet<string> existing = new(existingPaths ?? [], StringComparer.Ordinal);

        public List<(string SourcePath, string DestinationPath)> MoveCalls { get; } = [];

        public bool Exists(string path) => existing.Contains(path);

        public void Move(string sourcePath, string destinationPath)
        {
            MoveCalls.Add((sourcePath, destinationPath));
            existing.Remove(sourcePath);
            existing.Add(destinationPath);
        }
    }

    private sealed class FakeClock(params DateTimeOffset[] values) : IClock
    {
        private readonly Queue<DateTimeOffset> values = new(values);

        public DateTimeOffset UtcNow => values.Dequeue();
    }
}
