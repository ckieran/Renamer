using Renamer.UI.Plans;

namespace Renamer.UI;

public partial class MainPage : ContentPage
{
    public MainPage(IPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
