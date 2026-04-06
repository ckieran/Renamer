using Renamer.UI.Plans;
using Renamer.UI.Services;

namespace Renamer.UI;

public partial class MainPage : ContentPage
{
    private readonly ThemeService themeService;

    public MainPage(IPlanViewModel viewModel, ThemeService themeService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.themeService = themeService;
    }

    private void OnLightThemeClicked(object? sender, EventArgs e) =>
        themeService.SetTheme(ThemeMode.Light);

    private void OnDarkThemeClicked(object? sender, EventArgs e) =>
        themeService.SetTheme(ThemeMode.Dark);

    private void OnSystemThemeClicked(object? sender, EventArgs e) =>
        themeService.SetTheme(ThemeMode.System);
}
