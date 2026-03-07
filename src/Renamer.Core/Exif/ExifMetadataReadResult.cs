namespace Renamer.Core.Exif;

public sealed record ExifMetadataReadResult
{
    public required ExifMetadataReadStatus Status { get; init; }

    public DateOnly? CaptureDate { get; init; }

    public static ExifMetadataReadResult Found(DateOnly captureDate) => new()
    {
        Status = ExifMetadataReadStatus.Found,
        CaptureDate = captureDate
    };

    public static ExifMetadataReadResult Missing() => new()
    {
        Status = ExifMetadataReadStatus.Missing,
        CaptureDate = null
    };

    public static ExifMetadataReadResult Invalid() => new()
    {
        Status = ExifMetadataReadStatus.Invalid,
        CaptureDate = null
    };

    public static ExifMetadataReadResult IoFailure() => new()
    {
        Status = ExifMetadataReadStatus.IoFailure,
        CaptureDate = null
    };
}

public enum ExifMetadataReadStatus
{
    Found,
    Missing,
    Invalid,
    IoFailure
}
