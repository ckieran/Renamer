using Renamer.UI.Plans;
using Renamer.UI.Services;

namespace Renamer.UI;

public partial class MainPage : ContentPage
{
    private readonly ThemeService themeService;
    private bool updatingTheme;

    public MainPage(IPlanViewModel viewModel, ThemeService themeService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        this.themeService = themeService;
        themeService.ThemeModeChanged += OnThemeModeChanged;
    }

    // Fired by ThemeService whenever the mode changes (including on first initialisation)
    private void OnThemeModeChanged(object? sender, ThemeMode mode) =>
        MainThread.BeginInvokeOnMainThread(() => ApplyModeToUi(mode));

    private void ApplyModeToUi(ThemeMode mode)
    {
        UpdateMenuCheckmarks(mode);
        UpdateRadioButtons(mode);
    }

    // ── Radio buttons ────────────────────────────────────────────────────────

    private void UpdateRadioButtons(ThemeMode mode)
    {
        updatingTheme = true;
        LightRadio.IsChecked  = mode == ThemeMode.Light;
        DarkRadio.IsChecked   = mode == ThemeMode.Dark;
        SystemRadio.IsChecked = mode == ThemeMode.System;
        updatingTheme = false;
    }

    private void OnThemeRadioChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (updatingTheme || !e.Value) return;

        var mode = sender switch
        {
            RadioButton r when ReferenceEquals(r, LightRadio)  => ThemeMode.Light,
            RadioButton r when ReferenceEquals(r, DarkRadio)   => ThemeMode.Dark,
            _                                                   => ThemeMode.System
        };
        themeService.SetTheme(mode);
    }

    // ── Menu bar items ───────────────────────────────────────────────────────

    private void UpdateMenuCheckmarks(ThemeMode mode)
    {
        LightThemeMenuItem.Text  = mode == ThemeMode.Light  ? "✓ Light Theme"    : "Light Theme";
        DarkThemeMenuItem.Text   = mode == ThemeMode.Dark   ? "✓ Dark Theme"     : "Dark Theme";
        SystemThemeMenuItem.Text = mode == ThemeMode.System ? "✓ System Default" : "System Default";
    }

    private void OnLightThemeClicked(object? sender, EventArgs e)  => themeService.SetTheme(ThemeMode.Light);
    private void OnDarkThemeClicked(object? sender, EventArgs e)   => themeService.SetTheme(ThemeMode.Dark);
    private void OnSystemThemeClicked(object? sender, EventArgs e) => themeService.SetTheme(ThemeMode.System);
}
