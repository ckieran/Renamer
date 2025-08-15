using Microsoft.Maui.Controls;

namespace Renamer.UI.Services
{
    public class ThemeService : IThemeService
    {
        private const string ThemeKey = "AppTheme";

        public void SetAppTheme(AppTheme theme)
        {
            Application.Current.UserAppTheme = theme;
            Preferences.Set(ThemeKey, theme.ToString());
        }

        public AppTheme GetAppTheme()
        {
            var themeStr = Preferences.Get(ThemeKey, AppTheme.Unspecified.ToString());
            if (Enum.TryParse(themeStr, out AppTheme theme))
                return theme;
            return AppTheme.Unspecified;
        }

        public void ToggleTheme()
        {
            var current = GetAppTheme();
            var newTheme = current == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
            SetAppTheme(newTheme);
        }
    }
}