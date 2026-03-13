using Renamer.Core.Contracts;
using Renamer.Core.Serialization;
using Renamer.UI.Plans;
using Microsoft.Extensions.Logging.Abstractions;

namespace Renamer.Tests.UI;

public sealed class PlanViewModelTests
{
    [Fact]
    public async Task LoadAsync_WithValidPlan_PopulatesSummaryAndOperations()
    {
        var viewModel = new PlanViewModel(
            new FakePlanSerializer(CreatePlan()),
            new FakePlanFilePicker(null),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/rename-plan.json"
        };

        await viewModel.LoadAsync();

        Assert.True(viewModel.IsLoaded);
        Assert.False(viewModel.HasError);
        Assert.Equal("Loaded 1 planned operation(s).", viewModel.StatusMessage);
        Assert.Equal(5, viewModel.SummaryItems.Count);
        var operation = Assert.Single(viewModel.Operations);
        Assert.Equal("/photos/Trip A", operation.SourcePath);
        Assert.Equal("2024-06-12 to 2024-06-14", operation.DateRangeText);
    }

    [Fact]
    public async Task LoadAsync_WithMissingPath_SetsActionableErrorState()
    {
        var viewModel = new PlanViewModel(
            new FakePlanSerializer(CreatePlan()),
            new FakePlanFilePicker(null),
            NullLogger<PlanViewModel>.Instance);

        await viewModel.LoadAsync();

        Assert.True(viewModel.HasError);
        Assert.Equal("Select a plan artifact path to load.", viewModel.ErrorMessage);
        Assert.Empty(viewModel.SummaryItems);
        Assert.Empty(viewModel.Operations);
    }

    [Fact]
    public async Task LoadAsync_WhenSerializerThrows_ShowsInlineErrorState()
    {
        var viewModel = new PlanViewModel(
            new ThrowingPlanSerializer(new InvalidDataException("broken plan")),
            new FakePlanFilePicker(null),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/broken-plan.json"
        };

        await viewModel.LoadAsync();

        Assert.True(viewModel.HasError);
        Assert.Contains("broken plan", viewModel.ErrorMessage);
        Assert.Equal("Plan preview unavailable.", viewModel.StatusMessage);
        Assert.Empty(viewModel.Operations);
    }

    [Fact]
    public async Task BrowseAsync_WhenPickerReturnsPath_UpdatesPlanPath()
    {
        var viewModel = new PlanViewModel(
            new FakePlanSerializer(CreatePlan()),
            new FakePlanFilePicker("/tmp/picked-plan.json"),
            NullLogger<PlanViewModel>.Instance);

        await viewModel.BrowseAsync();

        Assert.Equal("/tmp/picked-plan.json", viewModel.PlanPath);
        Assert.Equal("Selected plan artifact: picked-plan.json", viewModel.StatusMessage);
    }

    [Fact]
    public async Task BrowseAsync_WhenPickerThrows_ShowsErrorState()
    {
        var viewModel = new PlanViewModel(
            new FakePlanSerializer(CreatePlan()),
            new ThrowingPlanFilePicker(new InvalidOperationException("picker unavailable")),
            NullLogger<PlanViewModel>.Instance);

        await viewModel.BrowseAsync();

        Assert.True(viewModel.HasError);
        Assert.Contains("picker unavailable", viewModel.ErrorMessage);
    }

    private static RenamePlan CreatePlan() =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            CreatedAtUtc = "2026-03-01T16:10:00Z",
            RootPath = "/photos",
            Operations =
            [
                new RenamePlanOperation
                {
                    OpId = "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    SourcePath = "/photos/Trip A",
                    PlannedDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
                    Reason = new RenamePlanReason
                    {
                        StartDate = "2024-06-12",
                        EndDate = "2024-06-14",
                        FilesConsidered = 12,
                        FilesSkippedMissingExif = 1
                    }
                }
            ],
            Summary = new RenamePlanSummary
            {
                OperationCount = 1,
                Warnings = 1
            }
        };

    private sealed class FakePlanSerializer(RenamePlan plan) : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => plan;

        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class ThrowingPlanSerializer(Exception exception) : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => throw exception;

        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class FakePlanFilePicker(string? selectedPath) : IPlanFilePicker
    {
        public Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(selectedPath);
    }

    private sealed class ThrowingPlanFilePicker(Exception exception) : IPlanFilePicker
    {
        public Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default) =>
            throw exception;
    }
}
