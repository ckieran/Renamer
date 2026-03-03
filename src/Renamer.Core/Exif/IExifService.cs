namespace Renamer.Core.Exif;

public interface IExifService
{
    ExifReadResult ReadCaptureDate(string filePath);
}
