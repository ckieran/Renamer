using Renamer.UI.Plans;

namespace Renamer.UI;

public partial class MainPage : ContentPage
{
	private readonly IPlanViewModel viewModel;

	public MainPage(IPlanViewModel viewModel)
	{
		this.viewModel = viewModel;
		InitializeComponent();
		BindingContext = viewModel;
	}

	private async void OnBrowseClicked(object? sender, EventArgs e)
	{
		await viewModel.BrowseAsync();
	}

	private async void OnLoadClicked(object? sender, EventArgs e) => await viewModel.LoadAsync();
}
