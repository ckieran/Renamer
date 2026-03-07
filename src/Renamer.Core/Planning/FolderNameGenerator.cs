namespace Renamer.Core.Planning;

public sealed class FolderNameGenerator : IFolderNameGenerator
{
    public string Generate(string sourceFolderName, DateOnly startDate, DateOnly endDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFolderName);

        var prefix = startDate == endDate
            ? startDate.ToString("yyyy-MM-dd")
            : $"{startDate:yyyy-MM-dd} - {endDate:yyyy-MM-dd}";

        return $"{prefix} - {sourceFolderName}";
    }
}
