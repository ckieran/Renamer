using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public class ExifService : IExifService
    {
        private static readonly string[] _supported = new[] { 
            ".jpg", ".jpeg", ".jpe", ".png", ".tiff", ".tif", ".heic", ".dng", ".cr2", ".nef", ".arw", ".raf"
        };

        public Task<IEnumerable<string>> GetSupportedExtensionsAsync()
        {
            return Task.FromResult(_supported.AsEnumerable());
        }

        public Task<bool> IsValidImageFileAsync(string filePath)
        {
            var ext = Path.GetExtension(filePath)?.ToLowerInvariant() ?? string.Empty;
            return Task.FromResult(_supported.Contains(ext));
        }

        public async Task<PhotoMetadata> ExtractMetadataAsync(string filePath)
        {
            if (!File.Exists(filePath)) return new PhotoMetadata { FileName = Path.GetFileName(filePath) };

            try
            {
                var directories = ImageMetadataReader.ReadMetadata(filePath);
                var subIfd = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                DateTime? capture = null;
                if (subIfd != null && subIfd.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out DateTime dt))
                {
                    capture = dt;
                }

                return new PhotoMetadata
                {
                    FileName = Path.GetFileName(filePath),
                    CaptureDate = capture
                };
            }
            catch
            {
                return new PhotoMetadata { FileName = Path.GetFileName(filePath) };
            }
        }

        public async Task<DateTime?> GetCaptureDateAsync(string filePath)
        {
            var meta = await ExtractMetadataAsync(filePath).ConfigureAwait(false);
            return meta.CaptureDate;
        }
    }
}
