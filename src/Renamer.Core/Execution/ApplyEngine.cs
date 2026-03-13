using Renamer.Core.Contracts;
using Renamer.Core.IO;
using Renamer.Core.Time;

namespace Renamer.Core.Execution;

public sealed class ApplyEngine : IApplyEngine
{
    public const string CompletedOutcome = "completed";
    public const string ConflictRetryLimitReachedOutcome = "conflictRetryLimitReached";

    private const string SchemaVersion = "1.0";
    private const string SuccessStatus = "success";
    private const string FailedStatus = "failed";
    private const string SkippedStatus = "skipped";

    private readonly IClock clock;
    private readonly IDirectoryMover directoryMover;
    private readonly IConflictRetryPolicy conflictRetryPolicy;

    public ApplyEngine()
        : this(new ConflictRetryPolicy(), new DirectoryMover(), new SystemClock())
    {
    }

    public ApplyEngine(IConflictRetryPolicy conflictRetryPolicy, IDirectoryMover directoryMover, IClock clock)
    {
        this.conflictRetryPolicy = conflictRetryPolicy ?? throw new ArgumentNullException(nameof(conflictRetryPolicy));
        this.directoryMover = directoryMover ?? throw new ArgumentNullException(nameof(directoryMover));
        this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public RenameReport Execute(RenamePlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);

        var startedAtUtc = clock.UtcNow;
        var results = new List<RenameReportResult>();

        foreach (var operation in plan.Operations)
        {
            var result = ExecuteOperation(operation);
            results.Add(result);

            if (result.Status == FailedStatus)
            {
                break;
            }
        }

        var finishedAtUtc = clock.UtcNow;

        return new RenameReport
        {
            Outcome = results.Any(result => result.Status == FailedStatus)
                ? ConflictRetryLimitReachedOutcome
                : CompletedOutcome,
            SchemaVersion = SchemaVersion,
            PlanId = plan.PlanId,
            StartedAtUtc = FormatUtc(startedAtUtc),
            FinishedAtUtc = FormatUtc(finishedAtUtc),
            Results = results,
            Summary = new RenameReportSummary
            {
                Success = results.Count(result => result.Status == SuccessStatus),
                Failed = results.Count(result => result.Status == FailedStatus),
                Skipped = results.Count(result => result.Status == SkippedStatus),
                Drifted = results.Count(result =>
                    result.Status == SuccessStatus &&
                    !string.Equals(result.PlannedDestinationPath, result.ActualDestinationPath, StringComparison.Ordinal))
            }
        };
    }

    private RenameReportResult ExecuteOperation(RenamePlanOperation operation)
    {
        var candidatePaths = conflictRetryPolicy.GetCandidatePaths(operation.PlannedDestinationPath);
        if (!directoryMover.Exists(operation.SourcePath))
        {
            var existingDestinationPath = candidatePaths.FirstOrDefault(directoryMover.Exists);
            if (!string.IsNullOrWhiteSpace(existingDestinationPath))
            {
                return new RenameReportResult
                {
                    OpId = operation.OpId,
                    SourcePath = operation.SourcePath,
                    PlannedDestinationPath = operation.PlannedDestinationPath,
                    ActualDestinationPath = existingDestinationPath,
                    Status = SkippedStatus,
                    Attempts = 1,
                    Warnings =
                    [
                        string.Equals(existingDestinationPath, operation.PlannedDestinationPath, StringComparison.Ordinal)
                            ? "Source path no longer exists; operation appears already completed."
                            : $"Source path no longer exists; operation appears already completed at{existingDestinationPath[operation.PlannedDestinationPath.Length..]}."
                    ],
                    Error = null
                };
            }
        }

        for (var index = 0; index < candidatePaths.Count; index++)
        {
            var candidatePath = candidatePaths[index];
            if (directoryMover.Exists(candidatePath))
            {
                continue;
            }

            directoryMover.Move(operation.SourcePath, candidatePath);

            return new RenameReportResult
            {
                OpId = operation.OpId,
                SourcePath = operation.SourcePath,
                PlannedDestinationPath = operation.PlannedDestinationPath,
                ActualDestinationPath = candidatePath,
                Status = SuccessStatus,
                Attempts = index + 1,
                Warnings = BuildWarnings(operation.PlannedDestinationPath, candidatePath),
                Error = null
            };
        }

        return new RenameReportResult
        {
            OpId = operation.OpId,
            SourcePath = operation.SourcePath,
            PlannedDestinationPath = operation.PlannedDestinationPath,
            ActualDestinationPath = null,
            Status = FailedStatus,
            Attempts = candidatePaths.Count,
            Warnings = [],
            Error = $"Destination conflict unresolved after {ConflictRetryPolicy.MaxSuffixRetries} suffix retries."
        };
    }

    private static string FormatUtc(DateTimeOffset timestamp) =>
        timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

    private static IReadOnlyList<string> BuildWarnings(string plannedDestinationPath, string actualDestinationPath)
    {
        if (string.Equals(plannedDestinationPath, actualDestinationPath, StringComparison.Ordinal))
        {
            return [];
        }

        var suffix = actualDestinationPath[plannedDestinationPath.Length..];
        return [$"Destination conflict; applied suffix{suffix}."];
    }
}
