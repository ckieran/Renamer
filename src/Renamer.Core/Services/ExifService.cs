using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Microsoft.Extensions.Logging;
using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public class ExifService : IExifService
    {
        private static readonly string[] _supported = new[]
        { ".jpg", ".jpeg", ".jpe", ".png", ".tiff", ".tif", ".heic", ".dng", ".cr2", ".nef", ".arw", ".raf" };
        private readonly ILogger<ExifService> _logger;

        public ExifService(ILogger<ExifService> logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            return _supported.AsEnumerable();
        }

        public bool IsValidImageFile(string filePath)
        {
            var ext = Path.GetExtension(filePath)?.ToLowerInvariant() ?? string.Empty;
            return _supported.Contains(ext);
        }

        public PhotoMetadata ExtractMetadata(string filePath)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to extract metadata for file {FilePath}", filePath);
                return new PhotoMetadata { FileName = Path.GetFileName(filePath) };
            }
        }

        public DateTime? GetCaptureDate(string filePath)
        {
            var meta = ExtractMetadata(filePath);
            return meta.CaptureDate;
        }
    }
}
