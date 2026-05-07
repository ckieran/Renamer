using Microsoft.Extensions.Logging.Abstractions;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Renamer.UI.Plans;
using Renamer.UI.Resources.Strings;

namespace Renamer.Tests.UI;

public sealed class PlanViewModelSummaryTests
{
    [Fact]
    public async Task PreviewStatChanges_AfterLoad_FormatsOperationCount()
    {
        var vm = MakeViewModel(operationCount: 2, warnings: 0);
        vm.PlanPath = "/tmp/plan.json";

        await vm.LoadAsync();

        Assert.Equal("2 changes", vm.PreviewStatChanges);
    }

    [Fact]
    public async Task PreviewStatNotes_WhenZero_UsesPlural()
    {
        var vm = MakeViewModel(operationCount: 1, warnings: 0);
        vm.PlanPath = "/tmp/plan.json";

        await vm.LoadAsync();

        Assert.Equal(string.Format(AppStrings.PreviewStatNotesPluralFormat, 0), vm.PreviewStatNotes);
    }

    [Fact]
    public async Task PreviewStatNotes_WhenOne_UsesSingular()
    {
        var vm = MakeViewModel(operationCount: 1, warnings: 1);
        vm.PlanPath = "/tmp/plan.json";

        await vm.LoadAsync();

        Assert.Equal(string.Format(AppStrings.PreviewStatNotesSingularFormat, 1), vm.PreviewStatNotes);
    }

    [Fact]
    public async Task PreviewStatNotes_WhenMany_UsesPlural()
    {
        var vm = MakeViewModel(operationCount: 1, warnings: 3);
        vm.PlanPath = "/tmp/plan.json";

        await vm.LoadAsync();

        Assert.Equal(string.Format(AppStrings.PreviewStatNotesPluralFormat, 3), vm.PreviewStatNotes);
    }

    [Fact]
    public async Task ApplyResultHeadline_WhenOne_UsesSingular()
    {
        var vm = MakeViewModelForApply(success: 1, skipped: 0, failed: 0);
        vm.PlanPath = "/tmp/plan.json";
        await vm.LoadAsync();

        await vm.ApplyAsync();

        Assert.Equal(string.Format(AppStrings.ApplyResultHeadlineSingular, 1), vm.ApplyResultHeadline);
    }

    [Fact]
    public async Task ApplyResultHeadline_WhenMany_UsesPlural()
    {
        var vm = MakeViewModelForApply(success: 3, skipped: 0, failed: 0);
        vm.PlanPath = "/tmp/plan.json";
        await vm.LoadAsync();

        await vm.ApplyAsync();

        Assert.Equal(string.Format(AppStrings.ApplyResultHeadlinePlural, 3), vm.ApplyResultHeadline);
    }

    [Fact]
    public async Task ApplyResultHeadline_WhenZero_UsesPlural()
    {
        var vm = MakeViewModelForApply(success: 0, skipped: 1, failed: 1);
        vm.PlanPath = "/tmp/plan.json";
        await vm.LoadAsync();

        await vm.ApplyAsync();

        Assert.Equal(string.Format(AppStrings.ApplyResultHeadlinePlural, 0), vm.ApplyResultHeadline);
    }

    [Fact]
    public async Task ApplyResultBreakdown_FormatsSkippedAndFailed()
    {
        var vm = MakeViewModelForApply(success: 2, skipped: 1, failed: 3);
        vm.PlanPath = "/tmp/plan.json";
        await vm.LoadAsync();

        await vm.ApplyAsync();

        Assert.Equal(string.Format(AppStrings.ApplyResultBreakdownFormat, 1, 3), vm.ApplyResultBreakdown);
    }

    private static PlanViewModel MakeViewModel(int operationCount, int warnings) =>
        new(
            new FakePlanBuilder(),
            new FakePlanSerializer(CreatePlan(operationCount, warnings)),
            new FakePlanFilePicker(),
            new FakeFolderPathPicker(),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateReport(0, 0, 0)),
            NullLogger<PlanViewModel>.Instance);

    private static PlanViewModel MakeViewModelForApply(int success, int skipped, int failed) =>
        new(
            new FakePlanBuilder(),
            new FakePlanSerializer(CreatePlan(1, 0)),
            new FakePlanFilePicker(),
            new FakeFolderPathPicker(),
            new FakeRootPathOpener(),
            new FakeApplyEngine(CreateReport(success, skipped, failed)),
            NullLogger<PlanViewModel>.Instance);

    private static RenamePlan CreatePlan(int operationCount, int warnings) =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "test-id",
            CreatedAtUtc = "2026-01-01T00:00:00Z",
            RootPath = "/photos",
            Operations = Enumerable.Range(0, operationCount)
                .Select(i => new RenamePlanOperation
                {
                    OpId = $"op-{i}",
                    SourcePath = $"/photos/folder-{i}",
                    PlannedDestinationPath = $"/photos/2024-01-01 - folder-{i}",
                    Reason = new RenamePlanReason
                    {
                        StartDate = "2024-01-01",
                        EndDate = "2024-01-01",
                        FilesConsidered = 5,
                        FilesSkippedMissingExif = i < warnings ? 1 : 0
                    }
                }).ToList(),
            Summary = new RenamePlanSummary { OperationCount = operationCount, Warnings = warnings }
        };

    private static RenameReport CreateReport(int success, int skipped, int failed) =>
        new()
        {
            SchemaVersion = "1.0",
            PlanId = "test-id",
            Outcome = "completed",
            StartedAtUtc = "2026-01-01T00:00:00Z",
            FinishedAtUtc = "2026-01-01T00:00:01Z",
            Results = [],
            Summary = new RenameReportSummary
            {
                Success = success,
                Skipped = skipped,
                Failed = failed,
                Drifted = 0
            }
        };

    private sealed class FakePlanBuilder : IPlanBuilder
    {
        public RenamePlan Build(string rootPath) => throw new NotImplementedException();
    }

    private sealed class FakePlanSerializer(RenamePlan plan) : IPlanSerializer
    {
        public RenamePlan Read(string inputPath) => plan;
        public void Write(string outputPath, RenamePlan plan) => throw new NotImplementedException();
    }

    private sealed class FakePlanFilePicker : IPlanFilePicker
    {
        public Task<string?> PickPlanPathAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(null);
    }

    private sealed class FakeFolderPathPicker : IFolderPathPicker
    {
        public Task<string?> PickFolderPathAsync(string title, CancellationToken cancellationToken = default) =>
            Task.FromResult<string?>(null);
    }

    private sealed class FakeRootPathOpener : IRootPathOpener
    {
        public Task OpenAsync(string directoryPath, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }

    private sealed class FakeApplyEngine(RenameReport report) : IApplyEngine
    {
        public RenameReport Execute(RenamePlan plan) => report;
    }
}
