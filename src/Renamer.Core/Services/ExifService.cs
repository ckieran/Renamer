using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Renamer.Core.Models;

namespace Renamer.Core.Services
{
    public class ExifService : IExifService
    {
        private static readonly string[] _supported = [ 
            ".jpg", ".jpeg", ".jpe", ".png", ".tiff", ".tif", ".heic", ".dng", ".cr2", ".nef", ".arw", ".raf"
        ];

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
            catch
            {
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
