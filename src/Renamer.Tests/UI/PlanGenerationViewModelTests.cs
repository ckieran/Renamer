using Microsoft.Extensions.Logging.Abstractions;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Renamer.UI.Plans;
using Renamer.UI.Resources.Strings;

namespace Renamer.Tests.UI;

public sealed class PlanGenerationViewModelTests
{
    [Fact]
    public async Task GeneratePlanAsync_WithValidInputs_WritesPlanAndPopulatesPreviewContext()
    {
        var existingDirectory = Path.GetTempPath();
        var plan = CreatePlan();
        var serializer = new RecordingPlanSerializer(plan);
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(plan),
            serializer,
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(),
            NullLogger<PlanViewModel>.Instance)
        {
            GenerationRootPath = existingDirectory,
            GenerationOutputDirectoryPath = existingDirectory,
            PlanFileName = "rename-plan.json"
        };

        await viewModel.GeneratePlanAsync();

        Assert.False(viewModel.IsGenerating);
        Assert.False(viewModel.HasGenerationError);
        Assert.True(viewModel.IsLoaded);
        Assert.Equal(Path.Combine(existingDirectory, "rename-plan.json"), viewModel.PlanPath);
        Assert.Equal(Path.Combine(existingDirectory, "rename-plan.json"), serializer.WrittenPath);
        Assert.Equal(existingDirectory, serializer.WrittenPlan?.RootPath);
        Assert.Equal(string.Format(AppStrings.PreviewStatusLoaded, 1), viewModel.StatusMessage);
        Assert.Equal(string.Format(AppStrings.GenerateStatusSuccess, "rename-plan.json"), viewModel.GenerationStatusMessage);
        Assert.Single(viewModel.Operations);
    }

    [Fact]
    public async Task GeneratePlanAsync_WithMissingRootPath_ShowsValidationError()
    {
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(CreatePlan()),
            new RecordingPlanSerializer(CreatePlan()),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(),
            NullLogger<PlanViewModel>.Instance)
        {
            GenerationOutputDirectoryPath = "/tmp/out",
            PlanFileName = "rename-plan.json"
        };

        await viewModel.GeneratePlanAsync();

        Assert.True(viewModel.HasGenerationError);
        Assert.Equal(AppStrings.GenerateErrorNoRootTitle, viewModel.GenerationErrorTitle);
    }

    [Fact]
    public async Task GeneratePlanAsync_WhenSerializerThrowsIOException_ShowsIoError()
    {
        var existingDirectory = Path.GetTempPath();
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(CreatePlan()),
            new ThrowingPlanSerializer(new IOException("read only volume")),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(),
            NullLogger<PlanViewModel>.Instance)
        {
            GenerationRootPath = existingDirectory,
            GenerationOutputDirectoryPath = existingDirectory,
            PlanFileName = "rename-plan.json"
        };

        await viewModel.GeneratePlanAsync();

        Assert.True(viewModel.HasGenerationError);
        Assert.Equal(AppStrings.GenerateErrorFileSystemTitle, viewModel.GenerationErrorTitle);
        Assert.Equal(AppStrings.GenerateErrorFileSystemMessage, viewModel.GenerationErrorMessage);
    }

    [Fact]
    public async Task GeneratePlanAsync_WithValidInputs_HandsOffPlanPathToApplyContext()
    {
        var existingDirectory = Path.GetTempPath();
        var plan = CreatePlan();
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(plan),
            new RecordingPlanSerializer(plan),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(),
            NullLogger<PlanViewModel>.Instance)
        {
            GenerationRootPath = existingDirectory,
            GenerationOutputDirectoryPath = existingDirectory,
            PlanFileName = "rename-plan.json"
        };

        await viewModel.GeneratePlanAsync();

        var expectedPath = Path.Combine(existingDirectory, "rename-plan.json");
        Assert.Equal(expectedPath, viewModel.PlanPath);
        Assert.True(viewModel.IsLoaded);
        Assert.True(viewModel.CanApply);
    }

    [Fact]
    public async Task BrowseGenerationRootPathAsync_WhenPickerReturnsPath_UpdatesRootPath()
    {
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(CreatePlan()),
            new RecordingPlanSerializer(CreatePlan()),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker("/photos"),
            new FakeRootPathOpener(),
            new FakeApplyEngine(),
            NullLogger<PlanViewModel>.Instance);

        await viewModel.BrowseGenerationRootPathAsync();

        Assert.Equal("/photos", viewModel.GenerationRootPath);
        Assert.Equal(string.Format(AppStrings.GenerateStatusRootSelected, "photos"), viewModel.GenerationStatusMessage);
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

    private sealed class FakePlanBuilder(RenamePlan plan) : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) =>
            new()
            {
                SchemaVersion = plan.SchemaVersion,
                PlanId = plan.PlanId,
                CreatedAtUtc = plan.CreatedAtUtc,
                RootPath = rootPath,
                Operations = plan.Operations,
                Summary = plan.Summary
            };
    }

    private sealed class RecordingPlanSerializer(RenamePlan planToRead) : IPlanSerializer
    {
        public string? WrittenPath { get; private set; }

        public RenamePlan? WrittenPlan { get; private set; }

        public RenamePlan Read(string inputPath) => planToRead;

        public void Write(string outputPath, RenamePlan plan)
        {
            WrittenPath = outputPath;
            WrittenPlan = plan;
        }
    }

    private sealed class ThrowingPlanSerializer(Exception exception) : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => throw new NotImplementedException();

        public void Write(string outputPath, RenamePlan plan) => throw exception;
    }

    private sealed class FakePlanFilePicker(string? selectedPath) : IPlanFilePicker
    {
        public Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default) => Task.FromResult(selectedPath);
    }

    private sealed class FakeFolderPathPicker(string? selectedPath) : IFolderPathPicker
    {
        public Task<string?> PickFolderPathAsync(string title, CancellationToken cancellationToken = default) => Task.FromResult(selectedPath);
    }

    private sealed class FakeRootPathOpener : IRootPathOpener
    {
        public Task OpenAsync(string directoryPath, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeApplyEngine : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => throw new NotImplementedException();
    }
}
