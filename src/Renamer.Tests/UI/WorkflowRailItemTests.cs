using Renamer.UI.Plans;
using Renamer.UI.Resources.Strings;

namespace Renamer.Tests.UI;

public sealed class WorkflowRailItemTests
{
    [Fact]
    public void TitleCharacters_ReturnsUppercasedNonWhitespaceChars()
    {
        var item = MakeItem(step: PlanWorkflowStep.GeneratePlan, title: "Build a plan");

        var chars = item.TitleCharacters;

        Assert.Equal(["B", "U", "I", "L", "D", "A", "P", "L", "A", "N"], chars);
    }

    [Fact]
    public void TitleCharacters_TrimsLeadingAndTrailingWhitespace()
    {
        var item = MakeItem(title: "  Plan  ");

        Assert.Equal(["P", "L", "A", "N"], item.TitleCharacters);
    }

    [Theory]
    [InlineData(PlanWorkflowStep.GeneratePlan, new[] { "P", "L", "A", "N" })]
    [InlineData(PlanWorkflowStep.PreviewPlan, new[] { "R", "E", "V", "I", "E", "W" })]
    [InlineData(PlanWorkflowStep.ApplyPlan, new[] { "R", "E", "N", "A", "M", "E" })]
    public void RailTitleCharacters_UsesShortWorkflowLabels(PlanWorkflowStep step, string[] expected)
    {
        var item = MakeItem(step: step, title: "Ignored long title");

        Assert.Equal(expected, item.RailTitleCharacters);
    }

    [Fact]
    public void StepNumber_MapsEnumToOrdinal()
    {
        Assert.Equal(1, MakeItem(step: PlanWorkflowStep.GeneratePlan).StepNumber);
        Assert.Equal(2, MakeItem(step: PlanWorkflowStep.PreviewPlan).StepNumber);
        Assert.Equal(3, MakeItem(step: PlanWorkflowStep.ApplyPlan).StepNumber);
    }

    [Fact]
    public void IndicatorState_DefaultsToNext()
    {
        var item = MakeItem();

        Assert.Equal("Next", item.IndicatorState);
    }

    [Fact]
    public void IndicatorState_WhenSelected_ReturnsNow()
    {
        var item = MakeItem();
        item.IsSelected = true;

        Assert.Equal("Now", item.IndicatorState);
    }

    [Fact]
    public void IndicatorState_WhenStatusDone_ReturnsDone()
    {
        var item = MakeItem();
        item.Status = PlanWorkflowStepStatus.Done;

        Assert.Equal("Done", item.IndicatorState);
    }

    [Fact]
    public void IndicatorState_WhenStatusError_ReturnsError()
    {
        var item = MakeItem();
        item.Status = PlanWorkflowStepStatus.Error;

        Assert.Equal("Error", item.IndicatorState);
    }

    [Fact]
    public void IndicatorState_WhenSelectedAndDone_ReturnsNow()
    {
        var item = MakeItem();
        item.IsSelected = true;
        item.Status = PlanWorkflowStepStatus.Done;

        Assert.Equal("Now", item.IndicatorState);
    }

    [Fact]
    public void IndicatorState_RaisesPropertyChanged_WhenIsSelectedChanges()
    {
        var item = MakeItem();
        var raised = new List<string?>();
        item.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        item.IsSelected = true;

        Assert.Contains(nameof(item.IndicatorState), raised);
    }

    [Fact]
    public void IndicatorState_RaisesPropertyChanged_WhenStatusChanges()
    {
        var item = MakeItem();
        var raised = new List<string?>();
        item.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        item.Status = PlanWorkflowStepStatus.Done;

        Assert.Contains(nameof(item.IndicatorState), raised);
    }

    private static PlanWorkflowStepItem MakeItem(
        PlanWorkflowStep step = PlanWorkflowStep.GeneratePlan,
        string title = "Plan") =>
        new(step, title, AppStrings.StepGenerateDescription);
}
