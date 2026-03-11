using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Exif;
using Renamer.Core.Time;

namespace Renamer.Core.Planning;

public sealed class PlanBuilder : IPlanBuilder
{
    private const string SchemaVersion = "1.0";

    private readonly IClock clock;
    private readonly IExifService exifService;
    private readonly IFolderDateRangeCalculator folderDateRangeCalculator;
    private readonly IFolderNameGenerator folderNameGenerator;
    private readonly IConflictRetryPolicy conflictRetryPolicy;

    public PlanBuilder(
        IExifService exifService,
        IFolderDateRangeCalculator folderDateRangeCalculator,
        IFolderNameGenerator folderNameGenerator,
        IConflictRetryPolicy conflictRetryPolicy,
        IClock clock)
    {
        this.exifService = exifService ?? throw new ArgumentNullException(nameof(exifService));
        this.folderDateRangeCalculator = folderDateRangeCalculator ?? throw new ArgumentNullException(nameof(folderDateRangeCalculator));
        this.folderNameGenerator = folderNameGenerator ?? throw new ArgumentNullException(nameof(folderNameGenerator));
        this.conflictRetryPolicy = conflictRetryPolicy ?? throw new ArgumentNullException(nameof(conflictRetryPolicy));
        this.clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public RenamePlan Build(string rootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rootPath);

        var fullRootPath = Path.GetFullPath(rootPath);
        var operations = new List<RenamePlanOperation>();
        var reservedDestinations = new HashSet<string>(StringComparer.Ordinal);
        var warnings = 0;

        foreach (var sourceDirectory in Directory.GetDirectories(fullRootPath).OrderBy(path => path, StringComparer.Ordinal))
        {
            var supportedFileResults = new List<ExifReadResult>();
            var filesConsidered = 0;
            var filesSkippedMissingExif = 0;

            foreach (var filePath in Directory.EnumerateFiles(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                var exifResult = exifService.ReadCaptureDate(filePath);
                if (!exifResult.IsSupportedFileType)
                {
                    continue;
                }

                filesConsidered++;
                if (exifResult.ShouldIncrementMissingExifCount)
                {
                    filesSkippedMissingExif++;
                    warnings++;
                }

                supportedFileResults.Add(exifResult);
            }

            var dateRange = folderDateRangeCalculator.Calculate(supportedFileResults);
            if (!dateRange.IsPlannable || dateRange.StartDate is null || dateRange.EndDate is null)
            {
                continue;
            }

            var folderName = Path.GetFileName(sourceDirectory);
            var plannedName = folderNameGenerator.Generate(folderName, dateRange.StartDate.Value, dateRange.EndDate.Value);
            var parentDirectory = Directory.GetParent(sourceDirectory)?.FullName
                ?? throw new InvalidOperationException($"Source directory '{sourceDirectory}' does not have a parent directory.");

            var baseDestinationPath = Path.Combine(parentDirectory, plannedName);
            var plannedDestinationPath = ResolvePlannedDestinationPath(baseDestinationPath, sourceDirectory, reservedDestinations);
            reservedDestinations.Add(plannedDestinationPath);

            operations.Add(new RenamePlanOperation
            {
                OpId = Guid.NewGuid().ToString(),
                SourcePath = sourceDirectory,
                PlannedDestinationPath = plannedDestinationPath,
                Reason = new RenamePlanReason
                {
                    StartDate = dateRange.StartDate.Value.ToString("yyyy-MM-dd"),
                    EndDate = dateRange.EndDate.Value.ToString("yyyy-MM-dd"),
                    FilesConsidered = filesConsidered,
                    FilesSkippedMissingExif = filesSkippedMissingExif
                }
            });
        }

        return new RenamePlan
        {
            SchemaVersion = SchemaVersion,
            PlanId = Guid.NewGuid().ToString(),
            CreatedAtUtc = clock.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"),
            RootPath = fullRootPath,
            Operations = operations,
            Summary = new RenamePlanSummary
            {
                OperationCount = operations.Count,
                Warnings = warnings
            }
        };
    }

    private string ResolvePlannedDestinationPath(string baseDestinationPath, string sourceDirectory, HashSet<string> reservedDestinations)
    {
        foreach (var candidatePath in conflictRetryPolicy.GetCandidatePaths(baseDestinationPath))
        {
            if (string.Equals(candidatePath, sourceDirectory, StringComparison.Ordinal))
            {
                continue;
            }

            if (reservedDestinations.Contains(candidatePath) || Directory.Exists(candidatePath))
            {
                continue;
            }

            return candidatePath;
        }

        throw new InvalidOperationException(
            $"Unable to determine a unique planned destination path for '{sourceDirectory}' after {ConflictRetryPolicy.MaxSuffixRetries} suffix retries.");
    }
}
