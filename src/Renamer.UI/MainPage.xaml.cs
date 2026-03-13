using Renamer.UI.Plans;
using Microsoft.Extensions.Logging;

namespace Renamer.UI;

public partial class MainPage : ContentPage
{
	private readonly IPlanViewModel viewModel;
    private readonly ILogger<MainPage> logger;

	public MainPage(IPlanViewModel viewModel, ILogger<MainPage> logger)
	{
		this.viewModel = viewModel;
        this.logger = logger;
		InitializeComponent();
		BindingContext = viewModel;
	}

	private async void OnBrowseClicked(object? sender, EventArgs e)
	{
        logger.LogInformation("Browse button clicked.");
		await viewModel.BrowseAsync();
	}

	private async void OnLoadClicked(object? sender, EventArgs e)
    {
        logger.LogInformation("Load preview button clicked.");
        await viewModel.LoadAsync();
    }

    private async void OnOpenRootPathTapped(object? sender, TappedEventArgs e)
    {
        logger.LogInformation("Root path tapped.");
        await viewModel.OpenRootPathAsync();
    }

    private async void OnApplyClicked(object? sender, EventArgs e)
    {
        logger.LogInformation("Apply button clicked.");
        await viewModel.ApplyAsync();
    }

    private async void OnBrowseGenerationRootClicked(object? sender, EventArgs e)
    {
        logger.LogInformation("Generation root browse button clicked.");
        await viewModel.BrowseGenerationRootPathAsync();
    }

    private async void OnBrowseGenerationOutputDirectoryClicked(object? sender, EventArgs e)
    {
        logger.LogInformation("Generation output directory browse button clicked.");
        await viewModel.BrowseGenerationOutputDirectoryAsync();
    }

    private async void OnGeneratePlanClicked(object? sender, EventArgs e)
    {
        logger.LogInformation("Generate plan button clicked.");
        await viewModel.GeneratePlanAsync();
    }
}
