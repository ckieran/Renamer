using Renamer.UI.Resources.Strings;

namespace Renamer.UI.Plans;

public sealed record PlanOperationItem(
    string OpId,
    string SourceName,
    string DestinationName,
    string SourcePath,
    string PlannedDestinationPath,
    string DateRangeText,
    string FileCountText,
    bool HasWarnings)
{
    public string FromDisplay => SourceName;
    public string ToDisplay => DestinationName;
    public string StatusPillText => HasWarnings ? AppStrings.PlanOperationStatusWarning : AppStrings.PlanOperationStatusOk;
    public string StatusPillColor => HasWarnings ? "#B45309" : "#166534";
}
