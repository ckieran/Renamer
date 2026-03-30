using Microsoft.Extensions.Logging.Abstractions;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Renamer.UI.Plans;
using Renamer.UI.Resources.Strings;

namespace Renamer.Tests.UI;

public sealed class ApplyFlowViewModelTests
{
    [Fact]
    public async Task ApplyAsync_WithSuccessfulReport_PopulatesSummaryAndResults()
    {
        var plan = CreatePlan();
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(plan),
            new FakePlanSerializer(plan),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateCompletedReport()),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/rename-plan.json"
        };

        await viewModel.LoadAsync();
        await viewModel.ApplyAsync();

        Assert.False(viewModel.IsApplying);
        Assert.False(viewModel.HasApplyError);
        Assert.True(viewModel.HasApplyReport);
        Assert.Equal("completed", viewModel.ApplyOutcomeText);
        Assert.Equal("1", viewModel.ApplySuccessCountText);
        Assert.Equal("0", viewModel.ApplyFailedCountText);
        Assert.Equal("1", viewModel.ApplyDriftedCountText);
        Assert.Equal(string.Format(AppStrings.ApplyStatusSuccess, 1, 0, 0), viewModel.ApplyStatusMessage);

        var result = Assert.Single(viewModel.ApplyResults);
        Assert.Equal("success", result.StatusText);
        Assert.Equal("/photos/2024-06-12 - 2024-06-14 - Trip A (1)", result.ActualDestinationPathText);
        Assert.Contains("suffix", result.WarningText);
    }

    [Fact]
    public async Task ApplyAsync_WithRetryAbortReport_ShowsAbortErrorAndRetainsReport()
    {
        var plan = CreatePlan();
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(plan),
            new FakePlanSerializer(plan),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateRetryAbortReport()),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/rename-plan.json"
        };

        await viewModel.LoadAsync();
        await viewModel.ApplyAsync();

        Assert.True(viewModel.HasApplyError);
        Assert.True(viewModel.HasApplyReport);
        Assert.Equal(AppStrings.ApplyErrorTitleRetryAbort, viewModel.ApplyErrorTitle);
        Assert.Equal(AppStrings.ApplyStatusPartial, viewModel.ApplyErrorMessage);
        Assert.Equal("1", viewModel.ApplyFailedCountText);
        Assert.Equal(AppStrings.ApplyStatusPartial, viewModel.ApplyStatusMessage);
    }

    [Fact]
    public async Task ApplyAsync_WhenPlanReloadFails_ShowsSchemaErrorState()
    {
        var serializer = new TwoStagePlanSerializer(CreatePlan(), new InvalidDataException("unsupported schema"));
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(CreatePlan()),
            serializer,
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateCompletedReport()),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/rename-plan.json"
        };

        await viewModel.LoadAsync();
        await viewModel.ApplyAsync();

        Assert.True(viewModel.HasApplyError);
        Assert.False(viewModel.HasApplyReport);
        Assert.Equal(AppStrings.ApplyErrorTitleInvalidPlan, viewModel.ApplyErrorTitle);
        Assert.Equal(AppStrings.ApplyErrorMessageInvalidPlan, viewModel.ApplyErrorMessage);
    }

    [Fact]
    public async Task ApplyAsync_WhenEngineThrowsIOException_ShowsIoErrorState()
    {
        var plan = CreatePlan();
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(plan),
            new FakePlanSerializer(plan),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new ThrowingApplyEngine(new DirectoryNotFoundException("source directory missing")),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/rename-plan.json"
        };

        await viewModel.LoadAsync();
        await viewModel.ApplyAsync();

        Assert.True(viewModel.HasApplyError);
        Assert.Equal(AppStrings.ApplyErrorTitleFileSystem, viewModel.ApplyErrorTitle);
        Assert.Equal(AppStrings.ApplyErrorMessageFileSystem, viewModel.ApplyErrorMessage);
    }

    [Fact]
    public async Task ApplyAsync_WithoutLoadedPlan_ShowsValidationError()
    {
        var plan = CreatePlan();
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(plan),
            new FakePlanSerializer(plan),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateCompletedReport()),
            NullLogger<PlanViewModel>.Instance);

        await viewModel.ApplyAsync();

        Assert.True(viewModel.HasApplyError);
        Assert.Equal(AppStrings.ApplyErrorTitleValidation, viewModel.ApplyErrorTitle);
        Assert.Equal(AppStrings.ApplyErrorMessageValidation, viewModel.ApplyErrorMessage);
    }

    [Fact]
    public async Task LoadAsync_WithValidPath_PopulatesStatusAndEnablesApply()
    {
        var plan = CreatePlan();
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(plan),
            new FakePlanSerializer(plan),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateCompletedReport()),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/rename-plan.json"
        };

        await viewModel.LoadAsync();

        Assert.True(viewModel.IsLoaded);
        Assert.True(viewModel.CanApply);
        Assert.Equal(string.Format(AppStrings.PreviewStatusLoaded, 1), viewModel.StatusMessage);
    }

    [Fact]
    public async Task LoadAsync_WhenSerializerThrowsIOException_ShowsErrorState()
    {
        var viewModel = new PlanViewModel(
            new FakePlanBuilder(CreatePlan()),
            new ThrowingOnReadPlanSerializer(new IOException("file not found")),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateCompletedReport()),
            NullLogger<PlanViewModel>.Instance)
        {
            PlanPath = "/tmp/rename-plan.json"
        };

        await viewModel.LoadAsync();

        Assert.True(viewModel.HasError);
        Assert.False(viewModel.CanApply);
        Assert.Equal(AppStrings.PreviewStatusLoadError, viewModel.ErrorMessage);
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

    private static RenameReport CreateCompletedReport() =>
        new()
        {
            Outcome = ApplyEngine.CompletedOutcome,
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            StartedAtUtc = "2026-03-01T16:11:00Z",
            FinishedAtUtc = "2026-03-01T16:11:01Z",
            Results =
            [
                new RenameReportResult
                {
                    OpId = "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    SourcePath = "/photos/Trip A",
                    PlannedDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
                    ActualDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A (1)",
                    Status = "success",
                    Attempts = 2,
                    Warnings = ["Destination conflict; applied suffix (1)."],
                    Error = null
                }
            ],
            Summary = new RenameReportSummary
            {
                Success = 1,
                Failed = 0,
                Skipped = 0,
                Drifted = 1
            }
        };

    private static RenameReport CreateRetryAbortReport() =>
        new()
        {
            Outcome = ApplyEngine.ConflictRetryLimitReachedOutcome,
            SchemaVersion = "1.0",
            PlanId = "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
            StartedAtUtc = "2026-03-01T16:11:00Z",
            FinishedAtUtc = "2026-03-01T16:11:05Z",
            Results =
            [
                new RenameReportResult
                {
                    OpId = "7c730a84-4b07-4f56-8758-9906cf488e6b",
                    SourcePath = "/photos/Trip A",
                    PlannedDestinationPath = "/photos/2024-06-12 - 2024-06-14 - Trip A",
                    ActualDestinationPath = null,
                    Status = "failed",
                    Attempts = 11,
                    Warnings = [],
                    Error = "Destination conflict unresolved after 10 suffix retries."
                }
            ],
            Summary = new RenameReportSummary
            {
                Success = 0,
                Failed = 1,
                Skipped = 0,
                Drifted = 0
            }
        };

    private sealed class FakePlanSerializer(RenamePlan plan) : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => plan;

        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class FakeApplyEngine(RenameReport report) : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => report;
    }

    private sealed class FakePlanBuilder(RenamePlan plan) : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => plan;
    }

    private sealed class TwoStagePlanSerializer(RenamePlan plan, Exception applyException) : IPlanSerializer
    {
        private int readCount;

        public RenamePlan Read(string inputPath)
        {
            readCount++;
            if (readCount == 1)
            {
                return plan;
            }

            throw applyException;
        }

        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class ThrowingApplyEngine(Exception exception) : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => throw exception;
    }

    private sealed class ThrowingOnReadPlanSerializer(Exception exception) : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => throw exception;

        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class FakePlanFilePicker(string? selectedPath) : IPlanFilePicker
    {
        public Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult(selectedPath);
    }

    private sealed class FakeFolderPathPicker(string? selectedPath) : IFolderPathPicker
    {
        public Task<string?> PickFolderPathAsync(string title, CancellationToken cancellationToken = default) =>
            Task.FromResult(selectedPath);
    }

    private sealed class FakeRootPathOpener : IRootPathOpener
    {
        public Task OpenAsync(string directoryPath, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
