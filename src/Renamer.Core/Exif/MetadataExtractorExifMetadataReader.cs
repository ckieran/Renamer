using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;

namespace Renamer.Core.Exif;

public sealed class MetadataExtractorExifMetadataReader(
    ILogger<MetadataExtractorExifMetadataReader> logger) : IExifMetadataReader
{
    private static readonly (Type DirectoryType, int TagType)[] DateTagPriority =
    [
        (typeof(ExifSubIfdDirectory), ExifDirectoryBase.TagDateTimeOriginal),
        (typeof(ExifSubIfdDirectory), ExifDirectoryBase.TagDateTimeDigitized),
        (typeof(ExifIfd0Directory), ExifDirectoryBase.TagDateTime)
    ];

    public MetadataExtractorExifMetadataReader()
        : this(NullLogger<MetadataExtractorExifMetadataReader>.Instance)
    {
    }

    public ExifMetadataReadResult ReadCaptureDate(string filePath)
    {
        try
        {
            var metadataDirectories = ImageMetadataReader.ReadMetadata(filePath);

            foreach (var dateTag in DateTagPriority)
            {
                var directory = metadataDirectories.FirstOrDefault(directory => directory.GetType() == dateTag.DirectoryType);
                if (directory is null || !directory.ContainsTag(dateTag.TagType))
                {
                    continue;
                }

                if (directory.TryGetDateTime(dateTag.TagType, out var captureDateTime))
                {
                    return ExifMetadataReadResult.Found(DateOnly.FromDateTime(captureDateTime));
                }

                logger.LogWarning(
                    "EXIF date tag {TagType} exists but could not be parsed for {FilePath}. Treating as invalid EXIF.",
                    dateTag.TagType,
                    filePath);
                return ExifMetadataReadResult.Invalid();
            }

            return ExifMetadataReadResult.Missing();
        }
        catch (ImageProcessingException ex)
        {
            logger.LogWarning(ex, "MetadataExtractor failed while reading EXIF metadata for {FilePath}.", filePath);
            return ExifMetadataReadResult.Invalid();
        }
        catch (IOException ex)
        {
            logger.LogWarning(ex, "IO failure while reading EXIF metadata for {FilePath}.", filePath);
            return ExifMetadataReadResult.IoFailure();
        }
        catch (UnauthorizedAccessException ex)
        {
            logger.LogWarning(ex, "Access denied while reading EXIF metadata for {FilePath}.", filePath);
            return ExifMetadataReadResult.IoFailure();
        }
    }
}
