using Renamer.UI.Plans;

namespace Renamer.UI.Views;

public partial class GenerateWorkspaceView : ContentView
{
    private bool advancedExpanded;

    public GenerateWorkspaceView()
    {
        InitializeComponent();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is IPlanViewModel vm)
        {
            vm.PropertyChanged += OnViewModelPropertyChanged;
            if (vm.HasAdvancedOverrides)
                SetAdvancedExpanded(true);
        }
    }

    private void OnViewModelPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IPlanViewModel.HasAdvancedOverrides) &&
            sender is IPlanViewModel vm && vm.HasAdvancedOverrides)
        {
            SetAdvancedExpanded(true);
        }
    }

    private void OnAdvancedToggleClicked(object? sender, EventArgs e) =>
        SetAdvancedExpanded(!advancedExpanded);

    private void SetAdvancedExpanded(bool expanded)
    {
        advancedExpanded = expanded;
        AdvancedPanel.IsVisible = expanded;
        AdvancedCollapsedHint.IsVisible = !expanded;
    }
}
