using CommunityToolkit.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace Renamer.UI.Plans;

public sealed class FolderPathPicker(IFolderPicker folderPicker, ILogger<FolderPathPicker> logger) : IFolderPathPicker
{
    public async Task<string?> PickFolderPathAsync(string title, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        logger.LogInformation("Opening native folder picker with title {Title}.", title);

        try
        {
            var result = await folderPicker.PickAsync(title, cancellationToken);
            if (!result.IsSuccessful)
            {
                if (result.Exception is OperationCanceledException)
                {
                    logger.LogInformation("Native folder picker canceled.");
                    return null;
                }

                throw result.Exception ?? new InvalidOperationException("Native folder picker did not return a successful result.");
            }

            logger.LogInformation("Native folder picker selected {FolderPath}.", result.Folder.Path);
            return result.Folder.Path;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("Native folder picker canceled.");
            return null;
        }
        catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException or IOException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Native folder picker failed.");
            throw;
        }
    }
}
