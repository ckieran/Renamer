using Microsoft.Extensions.Logging.Abstractions;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Renamer.UI.Plans;

namespace Renamer.Tests.UI;

public sealed class StepperShellViewModelTests
{
    [Fact]
    public void Constructor_DefaultsToGenerateStep()
    {
        var viewModel = CreateViewModel();

        Assert.Equal(PlanWorkflowStep.GeneratePlan, viewModel.ActiveStep);
        Assert.True(viewModel.IsGenerateStepActive);
        Assert.False(viewModel.IsPreviewStepActive);
        Assert.False(viewModel.IsApplyStepActive);
        Assert.True(viewModel.GenerateStep.IsSelected);
        Assert.Equal(PlanWorkflowStepStatus.NeedsInfo, viewModel.GenerateStep.Status);
        Assert.Equal(PlanWorkflowStepStatus.NeedsInfo, viewModel.PreviewStep.Status);
        Assert.Equal(PlanWorkflowStepStatus.NeedsInfo, viewModel.ApplyStep.Status);
    }

    [Fact]
    public void SelectStep_AllowsNonSequentialNavigation()
    {
        var viewModel = CreateViewModel();

        viewModel.SelectStep(PlanWorkflowStep.ApplyPlan);

        Assert.Equal(PlanWorkflowStep.ApplyPlan, viewModel.ActiveStep);
        Assert.False(viewModel.GenerateStep.IsSelected);
        Assert.False(viewModel.PreviewStep.IsSelected);
        Assert.True(viewModel.ApplyStep.IsSelected);
        Assert.True(viewModel.IsApplyStepActive);
    }

    [Fact]
    public async Task GeneratePlanAsync_MarksGenerateAndPreviewStepsDone()
    {
        var existingDirectory = Path.GetTempPath();
        var plan = CreatePlan();
        var viewModel = CreateViewModel(
            new FakePlanBuilder(plan),
            new RecordingPlanSerializer(plan));
        viewModel.GenerationRootPath = existingDirectory;
        viewModel.GenerationOutputDirectoryPath = existingDirectory;
        viewModel.PlanFileName = "rename-plan.json";

        await viewModel.GeneratePlanAsync();

        Assert.Equal(PlanWorkflowStepStatus.Done, viewModel.GenerateStep.Status);
        Assert.Equal(PlanWorkflowStepStatus.Done, viewModel.PreviewStep.Status);
        Assert.Equal(PlanWorkflowStepStatus.NeedsInfo, viewModel.ApplyStep.Status);
    }

    [Fact]
    public async Task LoadAsync_WhenPlanLoadFails_MarksPreviewStepError()
    {
        var viewModel = CreateViewModel(
            planSerializer: new ThrowingPlanSerializer(new InvalidDataException("broken plan")));
        viewModel.PlanPath = "/tmp/broken-plan.json";

        await viewModel.LoadAsync();

        Assert.Equal(PlanWorkflowStepStatus.Error, viewModel.PreviewStep.Status);
    }

    [Fact]
    public async Task ApplyAsync_WithCompletedReport_MarksApplyStepDone()
    {
        var plan = CreatePlan();
        var viewModel = CreateViewModel(
            new FakePlanBuilder(plan),
            new RecordingPlanSerializer(plan),
            applyEngine: new FakeApplyEngine(CreateCompletedReport()));
        viewModel.PlanPath = "/tmp/rename-plan.json";

        await viewModel.LoadAsync();
        await viewModel.ApplyAsync();

        Assert.Equal(PlanWorkflowStepStatus.Done, viewModel.ApplyStep.Status);
    }

    [Fact]
    public async Task ApplyAsync_WithRetryAbortReport_MarksApplyStepError()
    {
        var plan = CreatePlan();
        var viewModel = CreateViewModel(
            new FakePlanBuilder(plan),
            new RecordingPlanSerializer(plan),
            applyEngine: new FakeApplyEngine(CreateRetryAbortReport()));
        viewModel.PlanPath = "/tmp/rename-plan.json";

        await viewModel.LoadAsync();
        await viewModel.ApplyAsync();

        Assert.Equal(PlanWorkflowStepStatus.Error, viewModel.ApplyStep.Status);
    }

    private static PlanViewModel CreateViewModel(
        IPlanBuilder? planBuilder = null,
        IPlanSerializer? planSerializer = null,
        IApplyEngine? applyEngine = null) =>
        new(
            planBuilder ?? new FakePlanBuilder(CreatePlan()),
            planSerializer ?? new RecordingPlanSerializer(CreatePlan()),
            new FakePlanFilePicker(null),
            new FakeFolderPathPicker(null),
            new FakeRootPathOpener(),
            applyEngine ?? new FakeApplyEngine(CreateCompletedReport()),
            NullLogger<PlanViewModel>.Instance);

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
        public RenamePlan Read(string inputPath) => planToRead;

        public void Write(string outputPath, RenamePlan plan)
        {
        }
    }

    private sealed class ThrowingPlanSerializer(Exception exception) : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => throw exception;

        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
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

    private sealed class FakeApplyEngine(RenameReport report) : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => report;
    }
}
