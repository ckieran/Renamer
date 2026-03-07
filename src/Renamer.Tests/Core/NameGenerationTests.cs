using Renamer.Core.Planning;

namespace Renamer.Tests.Core;

public sealed class NameGenerationTests
{
    [Fact]
    public void Generate_SingleDay_FormatsDateThenOriginalFolderName()
    {
        var sut = new FolderNameGenerator();

        var result = sut.Generate("Trip A", new DateOnly(2024, 6, 12), new DateOnly(2024, 6, 12));

        Assert.Equal("2024-06-12 - Trip A", result);
    }

    [Fact]
    public void Generate_MultiDay_FormatsStartAndEndDatesThenOriginalFolderName()
    {
        var sut = new FolderNameGenerator();

        var result = sut.Generate("Trip A", new DateOnly(2024, 6, 12), new DateOnly(2024, 6, 14));

        Assert.Equal("2024-06-12 - 2024-06-14 - Trip A", result);
    }

    [Fact]
    public void Generate_PreservesSpacesAndPunctuationInFolderName()
    {
        var sut = new FolderNameGenerator();

        var result = sut.Generate("Trip, Day 1! (RAW + JPG)", new DateOnly(2024, 1, 5), new DateOnly(2024, 1, 5));

        Assert.Equal("2024-01-05 - Trip, Day 1! (RAW + JPG)", result);
    }

    [Fact]
    public void Generate_EmptyFolderName_ThrowsArgumentException()
    {
        var sut = new FolderNameGenerator();

        Assert.Throws<ArgumentException>(() => sut.Generate(string.Empty, new DateOnly(2024, 1, 5), new DateOnly(2024, 1, 5)));
    }
}
