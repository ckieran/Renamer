namespace Renamer.UI.Plans;

public sealed record PlanOperationItem(
    string OpId,
    string SourceName,
    string DestinationName,
    string SourcePath,
    string PlannedDestinationPath,
    string DateRangeText,
    string FileCountText);
