namespace Renamer.UI.Views;

public partial class ApplyWorkspaceView : ContentView
{
    private bool detailsExpanded;

    public ApplyWorkspaceView()
    {
        InitializeComponent();
    }

    private void OnDetailsToggleClicked(object? sender, EventArgs e)
    {
        detailsExpanded = !detailsExpanded;
        DetailsPanel.IsVisible = detailsExpanded;
    }
}
