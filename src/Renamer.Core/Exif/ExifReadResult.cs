namespace Renamer.Core.Exif;

public sealed record ExifReadResult
{
    public required bool IsSupportedFileType { get; init; }

    public DateOnly? CaptureDate { get; init; }

    public ExifReadWarning? Warning { get; init; }

    public bool ShouldIncrementMissingExifCount => Warning is ExifReadWarning.MissingExif or ExifReadWarning.InvalidExif;

    public static ExifReadResult Supported(DateOnly captureDate) => new()
    {
        IsSupportedFileType = true,
        CaptureDate = captureDate,
        Warning = null
    };

    public static ExifReadResult Unsupported() => new()
    {
        IsSupportedFileType = false,
        CaptureDate = null,
        Warning = null
    };

    public static ExifReadResult MissingExif() => new()
    {
        IsSupportedFileType = true,
        CaptureDate = null,
        Warning = ExifReadWarning.MissingExif
    };

    public static ExifReadResult InvalidExif() => new()
    {
        IsSupportedFileType = true,
        CaptureDate = null,
        Warning = ExifReadWarning.InvalidExif
    };
}
