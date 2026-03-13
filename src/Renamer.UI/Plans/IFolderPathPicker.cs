namespace Renamer.UI.Plans;

public interface IFolderPathPicker
{
    Task<string?> PickFolderPathAsync(string title, CancellationToken cancellationToken = default);
}
