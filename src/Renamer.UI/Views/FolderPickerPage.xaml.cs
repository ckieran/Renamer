using Microsoft.Maui.Controls;
using Renamer.UI.ViewModels;
using Renamer.UI.Services;

namespace Renamer.UI.Views;

public partial class FolderPickerPage : ContentPage
{
    private readonly IThemeService _themeService;

    public FolderPickerPage()
    {
        InitializeComponent();
        _themeService = Application.Current?.Handler?.MauiContext?.Services.GetService<IThemeService>() ?? new ThemeService();
        var currentTheme = _themeService.GetAppTheme();
        DarkModeSwitch.IsToggled = currentTheme == AppTheme.Dark;
        BindingContext = new FolderPickerViewModel();
    }

    private void OnDarkModeToggled(object sender, ToggledEventArgs e)
    {
        _themeService.SetAppTheme(e.Value ? AppTheme.Dark : AppTheme.Light);
    }
}
