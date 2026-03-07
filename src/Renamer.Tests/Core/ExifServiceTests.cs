using Renamer.Core.Exif;

namespace Renamer.Tests.Core;

public sealed class ExifServiceTests
{
    [Theory]
    [InlineData("photo.jpg")]
    [InlineData("photo.jpeg")]
    [InlineData("photo.nef")]
    [InlineData("photo.JPEG")]
    public void ReadCaptureDate_SupportedExtensions_ReturnsCaptureDate(string fileName)
    {
        var expectedDate = new DateOnly(2024, 6, 14);
        var metadataReader = new FakeExifMetadataReader(ExifMetadataReadResult.Found(expectedDate));
        var sut = new ExifService(metadataReader);

        var result = sut.ReadCaptureDate(fileName);

        Assert.True(result.IsSupportedFileType);
        Assert.Equal(expectedDate, result.CaptureDate);
        Assert.Null(result.Warning);
        Assert.False(result.ShouldIncrementMissingExifCount);
        Assert.Equal(1, metadataReader.CallCount);
    }

    [Fact]
    public void ReadCaptureDate_UnsupportedExtension_SkipsExifRead()
    {
        var metadataReader = new FakeExifMetadataReader(ExifMetadataReadResult.Found(new DateOnly(2024, 1, 1)));
        var sut = new ExifService(metadataReader);

        var result = sut.ReadCaptureDate("photo.png");

        Assert.False(result.IsSupportedFileType);
        Assert.Null(result.CaptureDate);
        Assert.Null(result.Warning);
        Assert.False(result.ShouldIncrementMissingExifCount);
        Assert.Equal(0, metadataReader.CallCount);
    }

    [Fact]
    public void ReadCaptureDate_MissingExif_ReturnsMissingWarning()
    {
        var metadataReader = new FakeExifMetadataReader(ExifMetadataReadResult.Missing());
        var sut = new ExifService(metadataReader);

        var result = sut.ReadCaptureDate("photo.nef");

        Assert.True(result.IsSupportedFileType);
        Assert.Null(result.CaptureDate);
        Assert.Equal(ExifReadWarning.MissingExif, result.Warning);
        Assert.True(result.ShouldIncrementMissingExifCount);
    }

    [Fact]
    public void ReadCaptureDate_InvalidExif_ReturnsInvalidWarning()
    {
        var metadataReader = new FakeExifMetadataReader(ExifMetadataReadResult.Invalid());
        var sut = new ExifService(metadataReader);

        var result = sut.ReadCaptureDate("photo.jpg");

        Assert.True(result.IsSupportedFileType);
        Assert.Null(result.CaptureDate);
        Assert.Equal(ExifReadWarning.InvalidExif, result.Warning);
        Assert.True(result.ShouldIncrementMissingExifCount);
    }

    [Fact]
    public void ReadCaptureDate_ReadFailure_ReturnsDedicatedWarningWithoutMissingExifIncrement()
    {
        var metadataReader = new FakeExifMetadataReader(ExifMetadataReadResult.IoFailure());
        var sut = new ExifService(metadataReader);

        var result = sut.ReadCaptureDate("photo.jpg");

        Assert.True(result.IsSupportedFileType);
        Assert.Null(result.CaptureDate);
        Assert.Equal(ExifReadWarning.ReadFailure, result.Warning);
        Assert.False(result.ShouldIncrementMissingExifCount);
    }

    [Fact]
    public void ReadCaptureDate_EmptyPath_ThrowsArgumentException()
    {
        var metadataReader = new FakeExifMetadataReader(ExifMetadataReadResult.Missing());
        var sut = new ExifService(metadataReader);

        Assert.Throws<ArgumentException>(() => sut.ReadCaptureDate(string.Empty));
    }

    private sealed class FakeExifMetadataReader(ExifMetadataReadResult result) : IExifMetadataReader
    {
        public int CallCount { get; private set; }

        public ExifMetadataReadResult ReadCaptureDate(string filePath)
        {
            CallCount++;
            return result;
        }
    }
}
