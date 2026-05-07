using Renamer.UI.Plans;
using Renamer.UI.Resources.Strings;

namespace Renamer.Tests.Plans;

public sealed class PlanOperationItemDisplayTests
{
    [Fact]
    public void FromDisplay_ReturnsSouceName()
    {
        var item = MakeItem(sourceName: "Trip A");

        Assert.Equal("Trip A", item.FromDisplay);
    }

    [Fact]
    public void ToDisplay_ReturnsDestinationName()
    {
        var item = MakeItem(destinationName: "2024-06-12 - Trip A");

        Assert.Equal("2024-06-12 - Trip A", item.ToDisplay);
    }

    [Fact]
    public void StatusPillText_WhenNoWarnings_ReturnsOk()
    {
        var item = MakeItem(hasWarnings: false);

        Assert.Equal(AppStrings.PlanOperationStatusOk, item.StatusPillText);
    }

    [Fact]
    public void StatusPillText_WhenHasWarnings_ReturnsWarning()
    {
        var item = MakeItem(hasWarnings: true);

        Assert.Equal(AppStrings.PlanOperationStatusWarning, item.StatusPillText);
    }

    [Fact]
    public void StatusPillColor_WhenNoWarnings_IsGreen()
    {
        var item = MakeItem(hasWarnings: false);

        Assert.Equal("#166534", item.StatusPillColor);
    }

    [Fact]
    public void StatusPillColor_WhenHasWarnings_IsAmber()
    {
        var item = MakeItem(hasWarnings: true);

        Assert.Equal("#B45309", item.StatusPillColor);
    }

    private static PlanOperationItem MakeItem(
        string sourceName = "Trip A",
        string destinationName = "2024-06-12 - Trip A",
        bool hasWarnings = false) =>
        new(
            OpId: "test-op-id",
            SourceName: sourceName,
            DestinationName: destinationName,
            SourcePath: "/photos/Trip A",
            PlannedDestinationPath: "/photos/2024-06-12 - Trip A",
            DateRangeText: "2024-06-12",
            FileCountText: "5 files checked, 0 without photo dates",
            HasWarnings: hasWarnings);
}
