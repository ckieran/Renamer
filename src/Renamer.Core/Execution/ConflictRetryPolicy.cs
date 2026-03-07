namespace Renamer.Core.Execution;

public sealed class ConflictRetryPolicy : IConflictRetryPolicy
{
    public const int MaxSuffixRetries = 10;
    public const int RetryLimitExitCode = 5;

    public IReadOnlyList<string> GetCandidatePaths(string plannedDestinationPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plannedDestinationPath);

        var candidates = new List<string>(MaxSuffixRetries + 1)
        {
            plannedDestinationPath
        };

        for (var suffix = 1; suffix <= MaxSuffixRetries; suffix++)
        {
            candidates.Add($"{plannedDestinationPath} ({suffix})");
        }

        return candidates;
    }

    public ConflictRetryResolution ResolveAvailableDestination(string plannedDestinationPath, Func<string, bool> destinationExists)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plannedDestinationPath);
        ArgumentNullException.ThrowIfNull(destinationExists);

        var candidates = GetCandidatePaths(plannedDestinationPath);

        for (var index = 0; index < candidates.Count; index++)
        {
            var candidatePath = candidates[index];
            if (!destinationExists(candidatePath))
            {
                return ConflictRetryResolution.Success(candidatePath, index + 1);
            }
        }

        return ConflictRetryResolution.RetryLimitExceeded(candidates.Count);
    }
}
