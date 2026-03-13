namespace Renamer.UI.Plans;

public interface IRootPathOpener
{
    Task OpenAsync(string directoryPath, CancellationToken cancellationToken = default);
}
