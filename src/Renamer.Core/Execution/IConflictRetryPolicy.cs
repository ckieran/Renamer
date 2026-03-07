namespace Renamer.Core.Execution;

public interface IConflictRetryPolicy
{
    IReadOnlyList<string> GetCandidatePaths(string plannedDestinationPath);

    ConflictRetryResolution ResolveAvailableDestination(string plannedDestinationPath, Func<string, bool> destinationExists);
}
