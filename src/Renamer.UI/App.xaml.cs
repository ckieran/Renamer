namespace Renamer.UI;

public partial class App : Application
{
        public App(Renamer.UI.Services.IThemeService themeService)
        {
                InitializeComponent();
                var theme = themeService.GetAppTheme();
                themeService.SetAppTheme(theme);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
                return new Window(new AppShell());
        }
}