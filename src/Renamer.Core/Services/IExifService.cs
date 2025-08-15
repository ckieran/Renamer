using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public interface IExifService
    {
        PhotoMetadata ExtractMetadata(string filePath);
        DateTime? GetCaptureDate(string filePath);
        bool IsValidImageFile(string filePath);
        IEnumerable<string> GetSupportedExtensions();
    }
}