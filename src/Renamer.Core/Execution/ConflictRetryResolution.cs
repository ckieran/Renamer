namespace Renamer.Core.Execution;

public sealed record ConflictRetryResolution
{
    public required bool Succeeded { get; init; }

    public required int Attempts { get; init; }

    public string? ResolvedDestinationPath { get; init; }

    public ConflictRetryFailure? Failure { get; init; }

    public int? SuggestedExitCode { get; init; }

    public bool ShouldAbortPlanExecution => Failure is not null;

    public static ConflictRetryResolution Success(string resolvedDestinationPath, int attempts) => new()
    {
        Succeeded = true,
        Attempts = attempts,
        ResolvedDestinationPath = resolvedDestinationPath,
        Failure = null,
        SuggestedExitCode = null
    };

    public static ConflictRetryResolution RetryLimitExceeded(int attempts) => new()
    {
        Succeeded = false,
        Attempts = attempts,
        ResolvedDestinationPath = null,
        Failure = ConflictRetryFailure.RetryLimitExceeded,
        SuggestedExitCode = ConflictRetryPolicy.RetryLimitExitCode
    };
}
