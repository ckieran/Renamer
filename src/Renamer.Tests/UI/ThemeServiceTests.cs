using Renamer.UI.Services;

namespace Renamer.Tests.UI;

public sealed class ThemeServiceTests
{
    [Fact]
    public void NextMode_FromLight_ReturnsDark() =>
        Assert.Equal(ThemeMode.Dark, ThemeCycler.NextMode(ThemeMode.Light));

    [Fact]
    public void NextMode_FromDark_ReturnsSystem() =>
        Assert.Equal(ThemeMode.System, ThemeCycler.NextMode(ThemeMode.Dark));

    [Fact]
    public void NextMode_FromSystem_ReturnsLight() =>
        Assert.Equal(ThemeMode.Light, ThemeCycler.NextMode(ThemeMode.System));

    [Fact]
    public void NextMode_CyclesFullRound()
    {
        var mode = ThemeMode.Light;
        mode = ThemeCycler.NextMode(mode);
        mode = ThemeCycler.NextMode(mode);
        mode = ThemeCycler.NextMode(mode);

        Assert.Equal(ThemeMode.Light, mode);
    }
}
