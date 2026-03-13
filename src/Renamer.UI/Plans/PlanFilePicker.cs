using Microsoft.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace Renamer.UI.Plans;

public sealed class PlanFilePicker(ILogger<PlanFilePicker> logger) : IPlanFilePicker
{
    private static readonly FilePickerFileType SupportedPlanFileTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, [".json"] },
        { DevicePlatform.MacCatalyst, ["public.json", "public.text"] }
    });

    public async Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Opening native plan file picker.");

        try
        {
            var fileResult = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Select rename-plan.json",
                FileTypes = SupportedPlanFileTypes
            });

            cancellationToken.ThrowIfCancellationRequested();

            if (fileResult is null)
            {
                logger.LogInformation("Native plan file picker returned no file.");
                return null;
            }

            logger.LogInformation("Native plan file picker selected {PlanPath}.", fileResult.FullPath);
            return fileResult.FullPath;
        }
        catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException or IOException)
        {
            logger.LogError(ex, "Native plan file picker failed.");
            throw;
        }
    }
}
