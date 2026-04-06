using Renamer.UI.Resources.Themes;

namespace Renamer.UI.Services;

public enum ThemeMode { System, Light, Dark }

public sealed class ThemeService
{
    private const string PreferenceKey = "AppThemeOverride";
    private bool followingSystem;
    private ResourceDictionary? currentThemeDict;

    public ThemeMode CurrentMode { get; private set; } = ThemeMode.System;

    public event EventHandler<ThemeMode>? ThemeModeChanged;

    public void Initialize()
    {
        var saved = Preferences.Default.Get(PreferenceKey, string.Empty);

        CurrentMode = saved switch
        {
            "Light" => ThemeMode.Light,
            "Dark"  => ThemeMode.Dark,
            _       => ThemeMode.System
        };

        followingSystem = CurrentMode == ThemeMode.System;
        ApplyTheme(ResolveEffectiveTheme());
        ThemeModeChanged?.Invoke(this, CurrentMode);

        Application.Current!.RequestedThemeChanged += OnOsThemeChanged;
    }

    public void SetTheme(ThemeMode mode)
    {
        CurrentMode = mode;
        followingSystem = mode == ThemeMode.System;

        if (mode == ThemeMode.System)
            Preferences.Default.Remove(PreferenceKey);
        else
            Preferences.Default.Set(PreferenceKey, mode.ToString());

        ApplyTheme(ResolveEffectiveTheme());
        ThemeModeChanged?.Invoke(this, CurrentMode);
    }

    private void OnOsThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        if (followingSystem)
            ApplyTheme(e.RequestedTheme);
    }

    private AppTheme ResolveEffectiveTheme() => CurrentMode switch
    {
        ThemeMode.Dark  => AppTheme.Dark,
        ThemeMode.Light => AppTheme.Light,
        _               => Application.Current!.RequestedTheme == AppTheme.Dark
                               ? AppTheme.Dark
                               : AppTheme.Light
    };

    private void ApplyTheme(AppTheme theme)
    {
        var resources = Application.Current!.Resources;

        // On first call currentThemeDict is null — find the dict loaded from App.xaml by type
        var toRemove = currentThemeDict
            ?? resources.MergedDictionaries.FirstOrDefault(d => d is DarkTheme or LightTheme);

        if (toRemove != null)
            resources.MergedDictionaries.Remove(toRemove);

        currentThemeDict = theme == AppTheme.Dark ? new DarkTheme() : new LightTheme();
        resources.MergedDictionaries.Add(currentThemeDict);
    }
}
