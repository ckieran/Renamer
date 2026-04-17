using Renamer.Core.Exif;

namespace Renamer.Core.Planning;

public sealed class FolderDateRangeCalculator : IFolderDateRangeCalculator
{
    public FolderDateRangeResult Calculate(IEnumerable<ExifReadResult> photoMetadata)
    {
        ArgumentNullException.ThrowIfNull(photoMetadata);

        DateOnly? minDate = null;
        DateOnly? maxDate = null;

        foreach (var metadata in photoMetadata)
        {
            if (metadata.CaptureDate is not { } captureDate) continue;

            minDate = minDate is null || captureDate < minDate ? captureDate : minDate;
            maxDate = maxDate is null || captureDate > maxDate ? captureDate : maxDate;
        }

        return minDate is { } startDate && maxDate is { } endDate
            ? FolderDateRangeResult.Plannable(startDate, endDate)
            : FolderDateRangeResult.NonPlannable();
    }
}
