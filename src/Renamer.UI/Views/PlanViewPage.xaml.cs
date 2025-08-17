using Renamer.Core.Services;
using Renamer.Core.Models;

namespace Renamer.UI.Views;

[QueryProperty(nameof(RootPath), "root")]
public partial class PlanViewPage : ContentPage
{
    private readonly IRenameService _renameService;
    public string RootPath { get; set; } = string.Empty;
    private RenamePlan? _currentPlan;

    public PlanViewPage(IRenameService renameService)
    {
        InitializeComponent();
        _renameService = renameService;
        ExecuteBtn.Clicked += ExecuteBtn_Clicked;
        RefreshBtn.Clicked += RefreshBtn_Clicked;
        PreviewSwitch.Toggled += PreviewSwitch_Toggled;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!string.IsNullOrEmpty(RootPath))
        {
            try
            {
                _currentPlan = await _renameService.GenerateRenamePlanAsync(RootPath);
                OperationsList.ItemsSource = _currentPlan.Operations;
                ExecuteBtn.IsEnabled = _currentPlan.Operations != null && _currentPlan.Operations.Count > 0;
                PlanInfoLabel.Text = $"Plan: {_currentPlan?.Operations.Count ?? 0} operations - Generated: {_currentPlan?.GeneratedAt.ToString("G") ?? "n/a"}";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private async void ExecuteBtn_Clicked(object? sender, EventArgs e)
    {
        try
        {
            BusyIndicator.IsVisible = true;
            BusyIndicator.IsRunning = true;
            ExecuteBtn.IsEnabled = false;
            StatusLabel.Text = "Executing...";

            // Confirm with the user before executing
            if (_currentPlan == null || _currentPlan.Operations.Count == 0)
            {
                await DisplayAlert("No operations", "There are no operations to execute.", "OK");
                return;
            }

            var confirm = await DisplayAlert("Confirm", $"Execute { _currentPlan.Operations.Count } operations? This cannot be undone.", "Execute", "Cancel");
            if (!confirm) return;

            var res = await _renameService.ExecuteRenamePlanAsync(_currentPlan);

            BusyIndicator.IsRunning = false;
            BusyIndicator.IsVisible = false;
            StatusLabel.Text = res.Success ? "Completed" : "Completed with errors";
            if (!res.Success && res.Errors.Any())
            {
                await DisplayAlert("Errors", string.Join('\n', res.Errors), "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            BusyIndicator.IsRunning = false;
            BusyIndicator.IsVisible = false;
            ExecuteBtn.IsEnabled = true;
        }
    }

    private async void RefreshBtn_Clicked(object? sender, EventArgs e)
    {
        try
        {
            BusyIndicator.IsRunning = true;
            BusyIndicator.IsVisible = true;
            StatusLabel.Text = "Refreshing...";
            _currentPlan = await _renameService.GenerateRenamePlanAsync(RootPath);
            OperationsList.ItemsSource = _currentPlan.Operations;
            ExecuteBtn.IsEnabled = _currentPlan.Operations != null && _currentPlan.Operations.Count > 0;
            PlanInfoLabel.Text = $"Plan: {_currentPlan?.Operations.Count ?? 0} operations - Generated: {_currentPlan?.GeneratedAt.ToString("G") ?? "n/a"}";
            StatusLabel.Text = "Ready";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            BusyIndicator.IsRunning = false;
            BusyIndicator.IsVisible = false;
        }
    }

    private void PreviewSwitch_Toggled(object? sender, ToggledEventArgs e)
    {
        if (_currentPlan == null) return;
        _currentPlan.IsPreview = e.Value;
        StatusLabel.Text = e.Value ? "Preview mode" : "Execute mode";
    }
}
