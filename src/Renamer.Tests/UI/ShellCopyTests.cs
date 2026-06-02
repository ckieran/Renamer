using Renamer.UI.Resources.Strings;

namespace Renamer.Tests.UI;

public sealed class ShellCopyTests
{
    [Fact]
    public void MainPageCopyUsesCompactAppShell()
    {
        Assert.Equal("Renamer", AppStrings.MainPageHeading);
        Assert.Equal("Plan · Review · Rename", AppStrings.MainPageDescription);
    }
}
