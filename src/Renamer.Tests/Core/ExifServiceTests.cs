using Microsoft.Extensions.Logging;
using Renamer.Core.Services;

namespace Renamer.Tests.Core
{
    public class ExifServiceTests
    {
        private readonly ILogger<ExifService> logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ExifService>();
        private const string SampleFileNameJpg = "sample_exif.jpg";
        private const string SampleFileNameDng = "tiny.dng";
        private static readonly byte[] _jpgWithExif = new byte[] {
            0xFF,0xD8,
            0xFF,0xE1,0x00,0x48, // APP1 length 72
            0x45,0x78,0x69,0x66,0x00,0x00, // "Exif\0\0"
            // TIFF header (little-endian)
            0x49,0x49,0x2A,0x00,0x08,0x00,0x00,0x00,
            // IFD0: 1 entry
            0x01,0x00,
            // Tag 0x8769 (ExifIFDPointer), type LONG (4), count=1, value=26
            0x69,0x87,0x04,0x00,0x01,0x00,0x00,0x00,0x1A,0x00,0x00,0x00,
            // next IFD offset
            0x00,0x00,0x00,0x00,
            // Exif IFD at offset 26: 1 entry
            0x01,0x00,
            // Tag 0x9003 DateTimeOriginal, type ASCII(2), count=20, value offset=44
            0x03,0x90,0x02,0x00,0x14,0x00,0x00,0x00,0x2C,0x00,0x00,0x00,
            // next IFD offset
            0x00,0x00,0x00,0x00,
            // ASCII string at offset 44: "2020:01:02 03:04:05\0"
            0x32,0x30,0x32,0x30,0x3A,0x30,0x31,0x3A,0x30,0x32,0x20,0x30,0x33,0x3A,0x30,0x34,0x3A,0x30,0x35,0x00,
            // EOI
            0xFF,0xD9
        };

        private string PrepareSampleFile(string fileName)
        {
            var dir = Path.Combine(AppContext.BaseDirectory, "SampleImages");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, fileName);
            if (!File.Exists(path))
            {
                // write same JPEG bytes for both .jpg and .dng (tests focus on metadata parsing)
                File.WriteAllBytes(path, _jpgWithExif);
            }
            return path;
        }
        [Fact]
        public void SupportedExtensions_ReturnsList()
        {
            
            var svc = new ExifService(logger);
            var exts = svc.GetSupportedExtensions();
            Assert.Contains(".jpg", exts);
            Assert.Contains(".dng", exts);
        }

        [Fact]
        public void IsValidImageFileAsync_Works()
        {
            var svc = new ExifService(logger);
            Assert.True(svc.IsValidImageFile("photo.jpg"));
            Assert.False(svc.IsValidImageFile("doc.txt"));
        }

        [Fact]
        public void ExtractMetadataAsync_FromSampleFiles_ReturnsCaptureDate()
        {
            var svc = new ExifService(logger);
            Console.WriteLine(AppContext.BaseDirectory);
            var jpgPath = PrepareSampleFile(SampleFileNameJpg);
            var dngPath = Path.Combine(AppContext.BaseDirectory, "SampleImages", SampleFileNameDng);

            var metaJpg = svc.ExtractMetadata(jpgPath);
            var metaDng = svc.ExtractMetadata(dngPath);

            Assert.NotNull(metaJpg);
            Assert.NotNull(metaJpg.CaptureDate);
            Assert.Equal(new System.DateTime(2020, 1, 2, 3, 4, 5), metaJpg.CaptureDate.Value);

            // TODO: Uncomment when DNG metadata extraction is implemented
            // Assert.NotNull(metaDng);
            // Assert.NotNull(metaDng.CaptureDate);
            // Assert.Equal(new System.DateTime(2020,1,2,3,4,5), metaDng.CaptureDate.Value);
        }
    }
}
