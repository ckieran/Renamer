using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public interface IExifService
    {
        Task<PhotoMetadata> ExtractMetadataAsync(string filePath);
        Task<DateTime?> GetCaptureDateAsync(string filePath);
        Task<bool> IsValidImageFileAsync(string filePath);
        Task<IEnumerable<string>> GetSupportedExtensionsAsync();
    }
}