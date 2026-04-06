using Renamer.UI.Services;

namespace Renamer.UI;

public partial class App : Application
{
    private readonly MainPage mainPage;

    public App(MainPage mainPage, ThemeService themeService)
    {
        InitializeComponent();
        themeService.Initialize();
        this.mainPage = mainPage;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(mainPage);
    }
}
