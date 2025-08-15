using Renamer.UI.Services;

namespace Renamer.UI;

public partial class MainPage : ContentPage
{
	int count = 0;
	private readonly IThemeService _themeService;

	public MainPage()
	{
		InitializeComponent();
		_themeService = Application.Current?.Handler.MauiContext?.Services.GetService<IThemeService>() ?? new ThemeService();
		var currentTheme = _themeService.GetAppTheme();
		DarkModeSwitch.IsToggled = currentTheme == AppTheme.Dark;
	}

	private void OnCounterClicked(object? sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

	private void OnDarkModeToggled(object sender, ToggledEventArgs e)
	{
		_themeService.SetAppTheme(e.Value ? AppTheme.Dark : AppTheme.Light);
	}
}
