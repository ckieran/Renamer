using Renamer.Core.Exif;
using Renamer.Core.Planning;

namespace Renamer.Tests.Core;

public sealed class DateRangeTests
{
    [Fact]
    public void Calculate_MixedValidAndMissingDates_ReturnsMinAndMaxFromValidDates()
    {
        var sut = new FolderDateRangeCalculator();
        ExifReadResult[] metadata =
        [
            ExifReadResult.MissingExif(),
            ExifReadResult.Supported(new DateOnly(2024, 6, 14)),
            ExifReadResult.InvalidExif(),
            ExifReadResult.Supported(new DateOnly(2024, 6, 12)),
            ExifReadResult.Supported(new DateOnly(2024, 6, 16))
        ];

        var result = sut.Calculate(metadata);

        Assert.True(result.IsPlannable);
        Assert.Equal(new DateOnly(2024, 6, 12), result.StartDate);
        Assert.Equal(new DateOnly(2024, 6, 16), result.EndDate);
    }

    [Fact]
    public void Calculate_SingleValidDate_ReturnsSameStartAndEndDate()
    {
        var sut = new FolderDateRangeCalculator();
        ExifReadResult[] metadata =
        [
            ExifReadResult.Unsupported(),
            ExifReadResult.Supported(new DateOnly(2024, 1, 5))
        ];

        var result = sut.Calculate(metadata);

        Assert.True(result.IsPlannable);
        Assert.Equal(new DateOnly(2024, 1, 5), result.StartDate);
        Assert.Equal(new DateOnly(2024, 1, 5), result.EndDate);
    }

    [Fact]
    public void Calculate_AllMissingDates_ReturnsNonPlannableResult()
    {
        var sut = new FolderDateRangeCalculator();
        ExifReadResult[] metadata =
        [
            ExifReadResult.MissingExif(),
            ExifReadResult.InvalidExif(),
            ExifReadResult.ReadFailure(),
            ExifReadResult.Unsupported()
        ];

        var result = sut.Calculate(metadata);

        Assert.False(result.IsPlannable);
        Assert.Null(result.StartDate);
        Assert.Null(result.EndDate);
    }

    [Fact]
    public void Calculate_NullMetadata_ThrowsArgumentNullException()
    {
        var sut = new FolderDateRangeCalculator();

        Assert.Throws<ArgumentNullException>(() => sut.Calculate(null!));
    }
}
