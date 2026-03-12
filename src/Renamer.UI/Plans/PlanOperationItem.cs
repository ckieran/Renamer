namespace Renamer.UI.Plans;

public sealed record PlanOperationItem(
    string OpId,
    string SourcePath,
    string PlannedDestinationPath,
    string DateRangeText,
    string FileCountText);
