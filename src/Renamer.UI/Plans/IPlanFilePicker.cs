namespace Renamer.UI.Plans;

public interface IPlanFilePicker
{
    Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default);
}
