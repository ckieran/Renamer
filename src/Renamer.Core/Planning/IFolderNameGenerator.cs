namespace Renamer.Core.Planning;

public interface IFolderNameGenerator
{
    string Generate(string sourceFolderName, DateOnly startDate, DateOnly endDate);
}
