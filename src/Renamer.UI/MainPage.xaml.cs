using Renamer.UI.Plans;
using Renamer.UI.Resources.Strings;
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
        themeService.ThemeModeChanged += OnThemeModeChanged;
    }

    private void OnThemeModeChanged(object? sender, ThemeMode mode) =>
        MainThread.BeginInvokeOnMainThread(() => ApplyModeToUi(mode));

    private void ApplyModeToUi(ThemeMode mode)
    {
        UpdateMenuCheckmarks(mode);
        UpdateThemePill(mode);
    }

    // ── Theme pill ───────────────────────────────────────────────────────────

    private void OnThemePillTapped(object? sender, TappedEventArgs e) =>
        themeService.CycleTheme();

    private void UpdateThemePill(ThemeMode mode)
    {
        var (glyph, label) = mode switch
        {
            ThemeMode.Light  => ("☀", AppStrings.ThemeToggleLight),
            ThemeMode.Dark   => ("☾", AppStrings.ThemeToggleDark),
            _                => ("⚙", AppStrings.ThemeToggleSystem)
        };

        ThemePillLabel.Text = $"{glyph} {label}";

        var accessibilityName = string.Format(AppStrings.ThemeToggleAccessibilityFormat, label);
        AutomationProperties.SetName(ThemePill, accessibilityName);
        ToolTipProperties.SetText(ThemePill, accessibilityName);
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
