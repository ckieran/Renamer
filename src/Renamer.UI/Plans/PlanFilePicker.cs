using Microsoft.Maui.Storage;

namespace Renamer.UI.Plans;

public sealed class PlanFilePicker : IPlanFilePicker
{
    private static readonly FilePickerFileType SupportedPlanFileTypes = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.WinUI, [".json"] },
        { DevicePlatform.MacCatalyst, ["public.json", "public.text"] }
    });

    public async Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default)
    {
        var fileResult = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select rename-plan.json",
            FileTypes = SupportedPlanFileTypes
        });

        cancellationToken.ThrowIfCancellationRequested();
        return fileResult?.FullPath;
    }
}
