namespace Renamer.Core.Planning;

public sealed record FolderDateRangeResult
{
    public required bool IsPlannable { get; init; }

    public DateOnly? StartDate { get; init; }

    public DateOnly? EndDate { get; init; }

    public static FolderDateRangeResult Plannable(DateOnly startDate, DateOnly endDate) => new()
    {
        IsPlannable = true,
        StartDate = startDate,
        EndDate = endDate
    };

    public static FolderDateRangeResult NonPlannable() => new()
    {
        IsPlannable = false,
        StartDate = null,
        EndDate = null
    };
}
