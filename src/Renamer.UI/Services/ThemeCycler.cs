namespace Renamer.UI.Services;

public static class ThemeCycler
{
    public static ThemeMode NextMode(ThemeMode current) => current switch
    {
        ThemeMode.Light => ThemeMode.Dark,
        ThemeMode.Dark  => ThemeMode.System,
        _               => ThemeMode.Light
    };
}
