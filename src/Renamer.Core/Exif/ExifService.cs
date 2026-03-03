namespace Renamer.Core.Exif;

public sealed class ExifService(IExifMetadataReader metadataReader) : IExifService
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".nef"
    };

    public ExifService() : this(new MetadataExtractorExifMetadataReader())
    {
    }

    public ExifReadResult ReadCaptureDate(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var extension = Path.GetExtension(filePath);
        if (!SupportedExtensions.Contains(extension))
        {
            return ExifReadResult.Unsupported();
        }

        var metadataResult = metadataReader.ReadCaptureDate(filePath);
        return metadataResult.Status switch
        {
            ExifMetadataReadStatus.Found when metadataResult.CaptureDate is { } captureDate => ExifReadResult.Supported(captureDate),
            ExifMetadataReadStatus.Missing => ExifReadResult.MissingExif(),
            ExifMetadataReadStatus.Invalid => ExifReadResult.InvalidExif(),
            _ => ExifReadResult.InvalidExif()
        };
    }
}
