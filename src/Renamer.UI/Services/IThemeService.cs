namespace Renamer.UI.Services
{
    public interface IThemeService
    {
        void SetAppTheme(AppTheme theme);
        AppTheme GetAppTheme();
        void ToggleTheme();
    }
}