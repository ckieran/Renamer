namespace Renamer.Core.Exif;

public interface IExifMetadataReader
{
    ExifMetadataReadResult ReadCaptureDate(string filePath);
}
