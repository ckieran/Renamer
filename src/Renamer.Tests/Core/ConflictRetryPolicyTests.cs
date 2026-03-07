using Renamer.Core.Execution;

namespace Renamer.Tests.Core;

public sealed class ConflictRetryPolicyTests
{
    [Fact]
    public void GetCandidatePaths_ReturnsBasePathThenDeterministicSuffixesThroughTen()
    {
        var sut = new ConflictRetryPolicy();

        var result = sut.GetCandidatePaths("/photos/2024-06-12 - Trip A");

        Assert.Equal(11, result.Count);
        Assert.Equal("/photos/2024-06-12 - Trip A", result[0]);
        Assert.Equal("/photos/2024-06-12 - Trip A (1)", result[1]);
        Assert.Equal("/photos/2024-06-12 - Trip A (2)", result[2]);
        Assert.Equal("/photos/2024-06-12 - Trip A (10)", result[10]);
    }

    [Fact]
    public void ResolveAvailableDestination_WhenConflictClears_UsesFirstAvailableSuffix()
    {
        var sut = new ConflictRetryPolicy();
        var existingPaths = new HashSet<string>(StringComparer.Ordinal)
        {
            "/photos/2024-06-12 - Trip A",
            "/photos/2024-06-12 - Trip A (1)"
        };

        var result = sut.ResolveAvailableDestination("/photos/2024-06-12 - Trip A", existingPaths.Contains);

        Assert.True(result.Succeeded);
        Assert.Equal(3, result.Attempts);
        Assert.Equal("/photos/2024-06-12 - Trip A (2)", result.ResolvedDestinationPath);
        Assert.False(result.ShouldAbortPlanExecution);
        Assert.Null(result.Failure);
        Assert.Null(result.SuggestedExitCode);
    }

    [Fact]
    public void ResolveAvailableDestination_WhenRetryLimitIsExceeded_ReturnsAbortFailureAndExitCodeFive()
    {
        var sut = new ConflictRetryPolicy();

        var result = sut.ResolveAvailableDestination("/photos/2024-06-12 - Trip A", _ => true);

        Assert.False(result.Succeeded);
        Assert.Equal(11, result.Attempts);
        Assert.Null(result.ResolvedDestinationPath);
        Assert.True(result.ShouldAbortPlanExecution);
        Assert.Equal(ConflictRetryFailure.RetryLimitExceeded, result.Failure);
        Assert.Equal(5, result.SuggestedExitCode);
    }

    [Fact]
    public void ResolveAvailableDestination_NullExistenceCheck_ThrowsArgumentNullException()
    {
        var sut = new ConflictRetryPolicy();

        Assert.Throws<ArgumentNullException>(() => sut.ResolveAvailableDestination("/photos/Trip A", null!));
    }
}
