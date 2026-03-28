using System.ComponentModel;

namespace Renamer.UI.Plans;

public interface IPlanViewModel : INotifyPropertyChanged
{
    PlanWorkflowStep ActiveStep { get; }

    PlanWorkflowStepItem GenerateStep { get; }

    PlanWorkflowStepItem PreviewStep { get; }

    PlanWorkflowStepItem ApplyStep { get; }

    bool IsGenerateStepActive { get; }

    bool IsPreviewStepActive { get; }

    bool IsApplyStepActive { get; }

    string GenerationRootPath { get; set; }

    string GenerationOutputDirectoryPath { get; set; }

    string PlanFileName { get; set; }

    string GeneratedPlanPathPreview { get; }

    string GenerationStatusMessage { get; }

    string? GenerationErrorTitle { get; }

    string? GenerationErrorMessage { get; }

    bool HasGenerationError { get; }

    bool IsGenerating { get; }

    bool CanGenerate { get; }

    string PlanPath { get; set; }

    string StatusMessage { get; }

    string? ErrorMessage { get; }

    bool IsIdle { get; }

    bool IsLoading { get; }

    bool IsLoaded { get; }

    bool HasError { get; }

    string RootPath { get; }

    string CreatedAtDisplay { get; }

    string OperationCountText { get; }

    string WarningCountText { get; }

    bool CanOpenRootPath { get; }

    bool CanApply { get; }

    bool IsApplying { get; }

    bool HasApplyReport { get; }

    bool HasApplyError { get; }

    string ApplyStatusMessage { get; }

    string? ApplyErrorTitle { get; }

    string? ApplyErrorMessage { get; }

    string ApplyOutcomeText { get; }

    string ApplyStartedAtDisplay { get; }

    string ApplyFinishedAtDisplay { get; }

    string ApplySuccessCountText { get; }

    string ApplyFailedCountText { get; }

    string ApplySkippedCountText { get; }

    string ApplyDriftedCountText { get; }

    System.Collections.ObjectModel.ObservableCollection<PlanOperationItem> Operations { get; }

    System.Collections.ObjectModel.ObservableCollection<ApplyResultItem> ApplyResults { get; }

    Task BrowseGenerationRootPathAsync(CancellationToken cancellationToken = default);

    Task BrowseGenerationOutputDirectoryAsync(CancellationToken cancellationToken = default);

    Task GeneratePlanAsync(CancellationToken cancellationToken = default);

    Task BrowseAsync(CancellationToken cancellationToken = default);

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task OpenRootPathAsync(CancellationToken cancellationToken = default);

    Task ApplyAsync(CancellationToken cancellationToken = default);

    void SelectStep(PlanWorkflowStep step);
}
