using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Planning;
using Renamer.Core.Serialization;
using Renamer.UI.Resources.Strings;

namespace Renamer.UI.Plans;

public sealed class PlanViewModel : IPlanViewModel
{
    private readonly IPlanBuilder planBuilder;
    private readonly IPlanSerializer planSerializer;
    private readonly IPlanFilePicker planFilePicker;
    private readonly IFolderPathPicker folderPathPicker;
    private readonly IRootPathOpener rootPathOpener;
    private readonly IApplyEngine applyEngine;
    private readonly ILogger<PlanViewModel> logger;
    private readonly PlanWorkflowStepItem generateStep;
    private readonly PlanWorkflowStepItem previewStep;
    private readonly PlanWorkflowStepItem applyStep;

    private RenamePlan? loadedPlan;
    private bool hasGeneratedPlan;
    private string generationRootPath = string.Empty;
    private string generationOutputDirectoryPath = string.Empty;
    private string planFileName = "rename-plan.json";
    private string generationStatusMessage = AppStrings.GenerateStatusDefault;
    private string? generationErrorTitle;
    private string? generationErrorMessage;
    private bool isGenerating;
    private string planPath = string.Empty;
    private string statusMessage = AppStrings.PreviewStatusDefault;
    private string? errorMessage;
    private string rootPath = string.Empty;
    private string createdAtDisplay = AppStrings.PreviewCreatedAtDefault;
    private string operationCountText = "0";
    private string warningCountText = "0";
    private string applyStatusMessage = AppStrings.ApplyStatusDefault;
    private string? applyErrorTitle;
    private string? applyErrorMessage;
    private string applyOutcomeText = AppStrings.ApplyStatusOutcomeDefault;
    private string applyStartedAtDisplay = AppStrings.ApplyStatusStartedDefault;
    private string applyFinishedAtDisplay = AppStrings.ApplyStatusFinishedDefault;
    private string applySuccessCountText = "0";
    private string applyFailedCountText = "0";
    private string applySkippedCountText = "0";
    private string applyDriftedCountText = "0";
    private PlanWorkflowStep activeStep = PlanWorkflowStep.GeneratePlan;
    private PlanViewState state = PlanViewState.Idle;
    private bool isApplying;
    private bool hasApplyReport;
    private bool isAutoUpdatingGenerationOutputDirectory;
    private string? autoFilledGenerationOutputDirectoryPath;

    public PlanViewModel(
        IPlanBuilder planBuilder,
        IPlanSerializer planSerializer,
        IPlanFilePicker planFilePicker,
        IFolderPathPicker folderPathPicker,
        IRootPathOpener rootPathOpener,
        IApplyEngine applyEngine,
        ILogger<PlanViewModel> logger)
    {
        this.planBuilder = planBuilder ?? throw new ArgumentNullException(nameof(planBuilder));
        this.planSerializer = planSerializer ?? throw new ArgumentNullException(nameof(planSerializer));
        this.planFilePicker = planFilePicker ?? throw new ArgumentNullException(nameof(planFilePicker));
        this.folderPathPicker = folderPathPicker ?? throw new ArgumentNullException(nameof(folderPathPicker));
        this.rootPathOpener = rootPathOpener ?? throw new ArgumentNullException(nameof(rootPathOpener));
        this.applyEngine = applyEngine ?? throw new ArgumentNullException(nameof(applyEngine));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        generateStep = new PlanWorkflowStepItem(
            PlanWorkflowStep.GeneratePlan,
            AppStrings.StepGenerateTitle,
            AppStrings.StepGenerateDescription);
        previewStep = new PlanWorkflowStepItem(
            PlanWorkflowStep.PreviewPlan,
            AppStrings.StepPreviewTitle,
            AppStrings.StepPreviewDescription);
        applyStep = new PlanWorkflowStepItem(
            PlanWorkflowStep.ApplyPlan,
            AppStrings.StepApplyTitle,
            AppStrings.StepApplyDescription);
        BrowseGenerationRootCommand = new AsyncCommand(BrowseGenerationRootPathAsync);
        BrowseGenerationOutputDirectoryCommand = new AsyncCommand(BrowseGenerationOutputDirectoryAsync);
        GeneratePlanCommand = new AsyncCommand(GeneratePlanAsync);
        BrowsePlanCommand = new AsyncCommand(BrowseAsync);
        LoadPreviewCommand = new AsyncCommand(LoadAsync);
        OpenRootPathCommand = new AsyncCommand(OpenRootPathAsync);
        ApplyCommand = new AsyncCommand(ApplyAsync);
        SelectStepCommand = new DelegateCommand<PlanWorkflowStep>(SelectStep);
        RefreshShellState();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public PlanWorkflowStep ActiveStep => activeStep;

    public PlanWorkflowStepItem GenerateStep => generateStep;

    public PlanWorkflowStepItem PreviewStep => previewStep;

    public PlanWorkflowStepItem ApplyStep => applyStep;

    public bool IsGenerateStepActive => ActiveStep == PlanWorkflowStep.GeneratePlan;

    public bool IsPreviewStepActive => ActiveStep == PlanWorkflowStep.PreviewPlan;

    public bool IsApplyStepActive => ActiveStep == PlanWorkflowStep.ApplyPlan;

    public string GenerationRootPath
    {
        get => generationRootPath;
        set
        {
            if (SetProperty(ref generationRootPath, value))
            {
                OnPropertyChanged(nameof(CanGenerate));
            }
        }
    }

    public string GenerationOutputDirectoryPath
    {
        get => generationOutputDirectoryPath;
        set
        {
            if (SetProperty(ref generationOutputDirectoryPath, value))
            {
                if (!isAutoUpdatingGenerationOutputDirectory)
                {
                    autoFilledGenerationOutputDirectoryPath = null;
                }

                OnPropertyChanged(nameof(GeneratedPlanPathPreview));
                OnPropertyChanged(nameof(CanGenerate));
            }
        }
    }

    public string PlanFileName
    {
        get => planFileName;
        set
        {
            if (SetProperty(ref planFileName, value))
            {
                OnPropertyChanged(nameof(GeneratedPlanPathPreview));
                OnPropertyChanged(nameof(CanGenerate));
            }
        }
    }

    public string GeneratedPlanPathPreview => BuildGeneratedPlanPathPreview();

    public string GenerationStatusMessage
    {
        get => generationStatusMessage;
        private set => SetProperty(ref generationStatusMessage, value);
    }

    public string? GenerationErrorTitle
    {
        get => generationErrorTitle;
        private set => SetProperty(ref generationErrorTitle, value);
    }

    public string? GenerationErrorMessage
    {
        get => generationErrorMessage;
        private set
        {
            if (SetProperty(ref generationErrorMessage, value))
            {
                OnPropertyChanged(nameof(HasGenerationError));
            }
        }
    }

    public bool HasGenerationError => !string.IsNullOrWhiteSpace(GenerationErrorMessage);

    public bool IsGenerating
    {
        get => isGenerating;
        private set
        {
            if (SetProperty(ref isGenerating, value))
            {
                OnPropertyChanged(nameof(CanGenerate));
            }
        }
    }

    public bool CanGenerate =>
        !IsGenerating &&
        !string.IsNullOrWhiteSpace(GenerationRootPath) &&
        !string.IsNullOrWhiteSpace(GenerationOutputDirectoryPath) &&
        !string.IsNullOrWhiteSpace(PlanFileName);

    public string PlanPath
    {
        get => planPath;
        set
        {
            if (SetProperty(ref planPath, value))
            {
                ResetPlanPreviewState();
                OnPropertyChanged(nameof(HasPlanPath));
            }
        }
    }

    public bool HasPlanPath => !string.IsNullOrWhiteSpace(PlanPath);

    public string StatusMessage
    {
        get => statusMessage;
        private set => SetProperty(ref statusMessage, value);
    }

    public string? ErrorMessage
    {
        get => errorMessage;
        private set => SetProperty(ref errorMessage, value);
    }

    public bool IsIdle => state == PlanViewState.Idle;

    public bool IsLoading => state == PlanViewState.Loading;

    public bool IsLoaded => state == PlanViewState.Loaded;

    public bool HasError => state == PlanViewState.Error;

    public string RootPath
    {
        get => rootPath;
        private set
        {
            if (SetProperty(ref rootPath, value))
            {
                OnPropertyChanged(nameof(CanOpenRootPath));
            }
        }
    }

    public string CreatedAtDisplay
    {
        get => createdAtDisplay;
        private set => SetProperty(ref createdAtDisplay, value);
    }

    public string OperationCountText
    {
        get => operationCountText;
        private set => SetProperty(ref operationCountText, value);
    }

    public string WarningCountText
    {
        get => warningCountText;
        private set => SetProperty(ref warningCountText, value);
    }

    public bool CanOpenRootPath => !string.IsNullOrWhiteSpace(RootPath);

    public bool CanApply => IsLoaded && !IsApplying;

    public ObservableCollection<PlanOperationItem> Operations { get; } = [];

    public ICommand BrowseGenerationRootCommand { get; }

    public ICommand BrowseGenerationOutputDirectoryCommand { get; }

    public ICommand GeneratePlanCommand { get; }

    public ICommand BrowsePlanCommand { get; }

    public ICommand LoadPreviewCommand { get; }

    public ICommand OpenRootPathCommand { get; }

    public ICommand ApplyCommand { get; }

    public ICommand SelectStepCommand { get; }

    public bool IsApplying
    {
        get => isApplying;
        private set
        {
            if (SetProperty(ref isApplying, value))
            {
                OnPropertyChanged(nameof(CanApply));
            }
        }
    }

    public bool HasApplyReport
    {
        get => hasApplyReport;
        private set => SetProperty(ref hasApplyReport, value);
    }

    public bool HasApplyError => !string.IsNullOrWhiteSpace(ApplyErrorMessage);

    public string ApplyStatusMessage
    {
        get => applyStatusMessage;
        private set => SetProperty(ref applyStatusMessage, value);
    }

    public string? ApplyErrorTitle
    {
        get => applyErrorTitle;
        private set => SetProperty(ref applyErrorTitle, value);
    }

    public string? ApplyErrorMessage
    {
        get => applyErrorMessage;
        private set
        {
            if (SetProperty(ref applyErrorMessage, value))
            {
                OnPropertyChanged(nameof(HasApplyError));
            }
        }
    }

    public string ApplyOutcomeText
    {
        get => applyOutcomeText;
        private set => SetProperty(ref applyOutcomeText, value);
    }

    public string ApplyStartedAtDisplay
    {
        get => applyStartedAtDisplay;
        private set => SetProperty(ref applyStartedAtDisplay, value);
    }

    public string ApplyFinishedAtDisplay
    {
        get => applyFinishedAtDisplay;
        private set => SetProperty(ref applyFinishedAtDisplay, value);
    }

    public string ApplySuccessCountText
    {
        get => applySuccessCountText;
        private set => SetProperty(ref applySuccessCountText, value);
    }

    public string ApplyFailedCountText
    {
        get => applyFailedCountText;
        private set => SetProperty(ref applyFailedCountText, value);
    }

    public string ApplySkippedCountText
    {
        get => applySkippedCountText;
        private set => SetProperty(ref applySkippedCountText, value);
    }

    public string ApplyDriftedCountText
    {
        get => applyDriftedCountText;
        private set => SetProperty(ref applyDriftedCountText, value);
    }

    public ObservableCollection<ApplyResultItem> ApplyResults { get; } = [];

    public async Task BrowseGenerationRootPathAsync(CancellationToken cancellationToken = default)
    {
        var selectedPath = await folderPathPicker.PickFolderPathAsync(AppStrings.GenerateFolderPickerRootTitle, cancellationToken);
        if (string.IsNullOrWhiteSpace(selectedPath))
        {
            GenerationStatusMessage = AppStrings.GenerateStatusRootCanceled;
            return;
        }

        GenerationRootPath = selectedPath;
        TryAutoFillGenerationOutputDirectory(selectedPath);
        ClearGenerationError();
        GenerationStatusMessage = string.Format(AppStrings.GenerateStatusRootSelected, Path.GetFileName(selectedPath));
    }

    public async Task BrowseGenerationOutputDirectoryAsync(CancellationToken cancellationToken = default)
    {
        var selectedPath = await folderPathPicker.PickFolderPathAsync(AppStrings.GenerateFolderPickerOutputTitle, cancellationToken);
        if (string.IsNullOrWhiteSpace(selectedPath))
        {
            GenerationStatusMessage = AppStrings.GenerateStatusOutputCanceled;
            return;
        }

        GenerationOutputDirectoryPath = selectedPath;
        autoFilledGenerationOutputDirectoryPath = null;
        ClearGenerationError();
        GenerationStatusMessage = string.Format(AppStrings.GenerateStatusOutputSelected, Path.GetFileName(selectedPath));
    }

    public async Task GeneratePlanAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(GenerationRootPath))
        {
            SetGenerationError(AppStrings.GenerateErrorNoRootTitle, AppStrings.GenerateErrorNoRootMessage);
            return;
        }

        if (!Directory.Exists(GenerationRootPath))
        {
            SetGenerationError(AppStrings.GenerateErrorInvalidRootTitle, string.Format(AppStrings.GenerateErrorInvalidRootMessage, GenerationRootPath));
            return;
        }

        if (string.IsNullOrWhiteSpace(GenerationOutputDirectoryPath))
        {
            SetGenerationError(AppStrings.GenerateErrorNoOutputTitle, AppStrings.GenerateErrorNoOutputMessage);
            return;
        }

        if (string.IsNullOrWhiteSpace(PlanFileName))
        {
            SetGenerationError(AppStrings.GenerateErrorNoFileNameTitle, AppStrings.GenerateErrorNoFileNameMessage);
            return;
        }

        string outputPath;
        try
        {
            outputPath = Path.Combine(GenerationOutputDirectoryPath, NormalizePlanFileName(PlanFileName));
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            logger.LogError(ex, "Plan file name is invalid.");
            SetGenerationError(AppStrings.GenerateErrorInvalidFileNameTitle, AppStrings.GenerateErrorInvalidFileNameMessage);
            return;
        }

        logger.LogInformation("Generating plan from root {RootPath} to {OutputPath}.", GenerationRootPath, outputPath);
        IsGenerating = true;
        ClearGenerationError();
        GenerationStatusMessage = AppStrings.GenerateStatusInProgress;

        try
        {
            var plan = await Task.Run(() => planBuilder.Build(GenerationRootPath), cancellationToken);
            await Task.Run(() => planSerializer.Write(outputPath, plan), cancellationToken);

            hasGeneratedPlan = true;
            loadedPlan = plan;
            SetGeneratedPlanPath(outputPath);
            ErrorMessage = null;
            Operations.Clear();
            ClearLoadedData();
            ResetApplyState();
            PopulateLoadedState(plan);
            GenerationStatusMessage = string.Format(AppStrings.GenerateStatusSuccess, Path.GetFileName(outputPath));
            logger.LogInformation("Generated plan with {OperationCount} operations at {OutputPath}.", plan.Operations.Count, outputPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Plan generation failed due to file system access.");
            SetGenerationError(AppStrings.GenerateErrorFileSystemTitle, AppStrings.GenerateErrorFileSystemMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Plan generation failed unexpectedly.");
            SetGenerationError(AppStrings.GenerateErrorUnexpectedTitle, AppStrings.GenerateErrorUnexpectedMessage);
        }
        finally
        {
            IsGenerating = false;
        }
    }

    public async Task BrowseAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Plan browse requested.");
        StatusMessage = AppStrings.PreviewStatusBrowseOpening;

        try
        {
            var selectedPath = await planFilePicker.PickPlanPathAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                logger.LogInformation("Plan selection canceled.");
                StatusMessage = AppStrings.PreviewStatusBrowseCanceled;
                return;
            }

            PlanPath = selectedPath;
            ErrorMessage = null;
            StatusMessage = string.Format(AppStrings.PreviewStatusBrowseSelected, Path.GetFileName(selectedPath));
            logger.LogInformation("Plan artifact selected: {PlanPath}", selectedPath);
        }
        catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException or IOException)
        {
            logger.LogError(ex, "Plan selection failed.");
            SetErrorState(AppStrings.PreviewStatusBrowseError);
        }
    }

    public async Task OpenRootPathAsync(CancellationToken cancellationToken = default)
    {
        if (!CanOpenRootPath)
        {
            logger.LogWarning("Root path open requested without a loaded plan.");
            return;
        }

        logger.LogInformation("Opening plan root path {RootPath}.", RootPath);

        try
        {
            await rootPathOpener.OpenAsync(RootPath, cancellationToken);
            StatusMessage = AppStrings.PreviewStatusRootOpened;
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or NotSupportedException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Unable to open plan root path {RootPath}.", RootPath);
            SetErrorState(AppStrings.PreviewStatusRootError);
        }
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(PlanPath))
        {
            logger.LogWarning("Plan load requested without a selected path.");
            SetErrorState(AppStrings.PreviewStatusNoPath);
            return;
        }

        logger.LogInformation("Loading plan preview from {PlanPath}.", PlanPath);
        SetState(PlanViewState.Loading);
        ErrorMessage = null;
        StatusMessage = AppStrings.PreviewStatusLoading;
        ClearLoadedData();
        Operations.Clear();
        ResetApplyState();

        try
        {
            var plan = await Task.Run(() => planSerializer.Read(PlanPath), cancellationToken);
            loadedPlan = plan;
            PopulateLoadedState(plan);
            logger.LogInformation("Loaded plan preview with {OperationCount} operations.", plan.Operations.Count);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or NotSupportedException)
        {
            logger.LogError(ex, "Unable to load plan preview from {PlanPath}.", PlanPath);
            loadedPlan = null;
            SetErrorState(AppStrings.PreviewStatusLoadError);
        }
    }

    public async Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(PlanPath) || loadedPlan is null || !IsLoaded)
        {
            logger.LogWarning("Apply requested without a loaded plan.");
            SetApplyError(AppStrings.ApplyErrorTitleValidation, AppStrings.ApplyErrorMessageValidation);
            return;
        }

        logger.LogInformation("Applying plan artifact {PlanPath}.", PlanPath);
        IsApplying = true;
        ApplyErrorTitle = null;
        ApplyErrorMessage = null;
        ApplyStatusMessage = AppStrings.ApplyStatusInProgress;
        ApplyResults.Clear();
        HasApplyReport = false;
        ResetApplySummary();

        try
        {
            var plan = await Task.Run(() => planSerializer.Read(PlanPath), cancellationToken);
            loadedPlan = plan;
            var report = await Task.Run(() => applyEngine.Execute(plan), cancellationToken);
            PopulateApplyState(report);
            LogSkippedResults(report);

            if (string.Equals(report.Outcome, ApplyEngine.ConflictRetryLimitReachedOutcome, StringComparison.Ordinal))
            {
                SetApplyError(AppStrings.ApplyErrorTitleRetryAbort, AppStrings.ApplyStatusPartial);
                ApplyStatusMessage = AppStrings.ApplyStatusPartial;
                return;
            }

            ApplyStatusMessage = string.Format(AppStrings.ApplyStatusSuccess, report.Summary.Success, report.Summary.Skipped, report.Summary.Failed);
        }
        catch (Exception ex) when (ex is InvalidDataException or NotSupportedException or System.Text.Json.JsonException)
        {
            logger.LogError(ex, "Plan schema validation failed during apply for {PlanPath}.", PlanPath);
            SetApplyError(AppStrings.ApplyErrorTitleInvalidPlan, AppStrings.ApplyErrorMessageInvalidPlan);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Apply failed due to file system access for {PlanPath}.", PlanPath);
            SetApplyError(AppStrings.ApplyErrorTitleFileSystem, AppStrings.ApplyErrorMessageFileSystem);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected apply failure for {PlanPath}.", PlanPath);
            SetApplyError(AppStrings.ApplyErrorTitleUnexpected, AppStrings.ApplyErrorMessageUnexpected);
        }
        finally
        {
            IsApplying = false;
        }
    }

    public void SelectStep(PlanWorkflowStep step)
    {
        if (activeStep == step)
        {
            return;
        }

        activeStep = step;
        RefreshShellState();
        OnPropertyChanged(nameof(ActiveStep));
        OnPropertyChanged(nameof(IsGenerateStepActive));
        OnPropertyChanged(nameof(IsPreviewStepActive));
        OnPropertyChanged(nameof(IsApplyStepActive));
    }

    private void PopulateLoadedState(RenamePlan plan)
    {
        RootPath = plan.RootPath;
        CreatedAtDisplay = FormatTimestamp(plan.CreatedAtUtc);
        OperationCountText = plan.Summary.OperationCount.ToString(CultureInfo.InvariantCulture);
        WarningCountText = plan.Summary.Warnings.ToString(CultureInfo.InvariantCulture);

        foreach (var operation in plan.Operations)
        {
            Operations.Add(new PlanOperationItem(
                operation.OpId,
                Path.GetFileName(operation.SourcePath),
                Path.GetFileName(operation.PlannedDestinationPath),
                operation.SourcePath,
                operation.PlannedDestinationPath,
                FormatDateRange(operation.Reason.StartDate, operation.Reason.EndDate),
                $"{operation.Reason.FilesConsidered} files, {operation.Reason.FilesSkippedMissingExif} missing EXIF"));
        }

        SetState(PlanViewState.Loaded);
        ErrorMessage = null;
        StatusMessage = string.Format(AppStrings.PreviewStatusLoaded, plan.Operations.Count);
        RefreshShellState();
    }

    private void PopulateApplyState(RenameReport report)
    {
        ApplyOutcomeText = report.Outcome;
        ApplyStartedAtDisplay = FormatTimestamp(report.StartedAtUtc);
        ApplyFinishedAtDisplay = FormatTimestamp(report.FinishedAtUtc);
        ApplySuccessCountText = report.Summary.Success.ToString(CultureInfo.InvariantCulture);
        ApplyFailedCountText = report.Summary.Failed.ToString(CultureInfo.InvariantCulture);
        ApplySkippedCountText = report.Summary.Skipped.ToString(CultureInfo.InvariantCulture);
        ApplyDriftedCountText = report.Summary.Drifted.ToString(CultureInfo.InvariantCulture);

        foreach (var result in report.Results)
        {
            ApplyResults.Add(new ApplyResultItem(
                result.OpId,
                Path.GetFileName(result.SourcePath),
                result.SourcePath,
                result.Status,
                result.PlannedDestinationPath,
                result.ActualDestinationPath ?? "Not moved",
                result.Warnings.Count == 0 ? "No warnings" : string.Join(" ", result.Warnings),
                result.Error ?? "No error"));
        }

        HasApplyReport = true;
        RefreshShellState();
    }

    private void SetErrorState(string message)
    {
        SetState(PlanViewState.Error);
        ErrorMessage = message;
        StatusMessage = AppStrings.PreviewStatusUnavailable;
        ClearLoadedData();
        Operations.Clear();
        loadedPlan = null;
        ResetApplyState();
        RefreshShellState();
    }

    private void ClearLoadedData()
    {
        RootPath = string.Empty;
        CreatedAtDisplay = AppStrings.PreviewCreatedAtDefault;
        OperationCountText = "0";
        WarningCountText = "0";
    }

    private void ResetPlanPreviewState()
    {
        loadedPlan = null;
        if (state != PlanViewState.Idle)
        {
            SetState(PlanViewState.Idle);
        }

        ErrorMessage = null;
        ClearLoadedData();
        Operations.Clear();
        ResetApplyState();
        StatusMessage = HasPlanPath
            ? AppStrings.PreviewStatusPathUpdated
            : AppStrings.PreviewStatusDefault;
        RefreshShellState();
    }

    private void ResetApplyState()
    {
        ApplyErrorTitle = null;
        ApplyErrorMessage = null;
        ApplyStatusMessage = AppStrings.ApplyStatusDefault;
        ApplyResults.Clear();
        HasApplyReport = false;
        IsApplying = false;
        ResetApplySummary();
        RefreshShellState();
    }

    private void ResetApplySummary()
    {
        ApplyOutcomeText = AppStrings.ApplyStatusOutcomeDefault;
        ApplyStartedAtDisplay = AppStrings.ApplyStatusStartedDefault;
        ApplyFinishedAtDisplay = AppStrings.ApplyStatusFinishedDefault;
        ApplySuccessCountText = "0";
        ApplyFailedCountText = "0";
        ApplySkippedCountText = "0";
        ApplyDriftedCountText = "0";
    }

    private void SetApplyError(string title, string message)
    {
        ApplyErrorTitle = title;
        ApplyErrorMessage = message;
        ApplyStatusMessage = AppStrings.ApplyStatusUnavailable;
        RefreshShellState();
    }

    private void SetGenerationError(string title, string message)
    {
        GenerationErrorTitle = title;
        GenerationErrorMessage = message;
        GenerationStatusMessage = AppStrings.GenerateStatusUnavailable;
        RefreshShellState();
    }

    private void ClearGenerationError()
    {
        GenerationErrorTitle = null;
        GenerationErrorMessage = null;
        RefreshShellState();
    }

    private void LogSkippedResults(RenameReport report)
    {
        foreach (var result in report.Results.Where(result => string.Equals(result.Status, "skipped", StringComparison.Ordinal)))
        {
            logger.LogInformation(
                "Apply skipped operation {OpId} because it appears already completed. Source={SourcePath} Destination={DestinationPath}",
                result.OpId,
                result.SourcePath,
                result.ActualDestinationPath ?? result.PlannedDestinationPath);
        }
    }

    private void SetGeneratedPlanPath(string outputPath)
    {
        if (string.Equals(planPath, outputPath, StringComparison.Ordinal))
        {
            return;
        }

        planPath = outputPath;
        OnPropertyChanged(nameof(PlanPath));
        OnPropertyChanged(nameof(HasPlanPath));
    }

    private void SetState(PlanViewState nextState)
    {
        if (state == nextState)
        {
            return;
        }

        state = nextState;
        OnPropertyChanged(nameof(IsIdle));
        OnPropertyChanged(nameof(IsLoading));
        OnPropertyChanged(nameof(IsLoaded));
        OnPropertyChanged(nameof(HasError));
        OnPropertyChanged(nameof(CanApply));
        RefreshShellState();
    }

    private void RefreshShellState()
    {
        generateStep.IsSelected = ActiveStep == PlanWorkflowStep.GeneratePlan;
        previewStep.IsSelected = ActiveStep == PlanWorkflowStep.PreviewPlan;
        applyStep.IsSelected = ActiveStep == PlanWorkflowStep.ApplyPlan;

        generateStep.Status = GetGenerateStepStatus();
        previewStep.Status = GetPreviewStepStatus();
        applyStep.Status = GetApplyStepStatus();
    }

    private PlanWorkflowStepStatus GetGenerateStepStatus()
    {
        if (HasGenerationError)
        {
            return PlanWorkflowStepStatus.Error;
        }

        return hasGeneratedPlan ? PlanWorkflowStepStatus.Done : PlanWorkflowStepStatus.NeedsInfo;
    }

    private PlanWorkflowStepStatus GetPreviewStepStatus()
    {
        if (HasError)
        {
            return PlanWorkflowStepStatus.Error;
        }

        return IsLoaded ? PlanWorkflowStepStatus.Done : PlanWorkflowStepStatus.NeedsInfo;
    }

    private PlanWorkflowStepStatus GetApplyStepStatus()
    {
        if (HasApplyError)
        {
            return PlanWorkflowStepStatus.Error;
        }

        return HasApplyReport ? PlanWorkflowStepStatus.Done : PlanWorkflowStepStatus.NeedsInfo;
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static string FormatTimestamp(string timestamp)
    {
        if (!DateTimeOffset.TryParse(
                timestamp,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var parsed))
        {
            return timestamp;
        }

        return parsed.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
    }

    private static string FormatDateRange(string startDate, string endDate) =>
        startDate == endDate ? startDate : $"{startDate} to {endDate}";

    private static string NormalizePlanFileName(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException($"Plan file name '{value}' contains invalid characters.");
        }

        return Path.HasExtension(trimmed) ? trimmed : $"{trimmed}.json";
    }

    private void TryAutoFillGenerationOutputDirectory(string rootPath)
    {
        if (!string.IsNullOrWhiteSpace(GenerationOutputDirectoryPath) &&
            !string.Equals(GenerationOutputDirectoryPath, autoFilledGenerationOutputDirectoryPath, StringComparison.Ordinal))
        {
            return;
        }

        isAutoUpdatingGenerationOutputDirectory = true;
        try
        {
            GenerationOutputDirectoryPath = rootPath;
            autoFilledGenerationOutputDirectoryPath = rootPath;
        }
        finally
        {
            isAutoUpdatingGenerationOutputDirectory = false;
        }
    }

    private string BuildGeneratedPlanPathPreview()
    {
        if (string.IsNullOrWhiteSpace(GenerationOutputDirectoryPath) || string.IsNullOrWhiteSpace(PlanFileName))
        {
            return string.Empty;
        }

        try
        {
            return Path.Combine(GenerationOutputDirectoryPath, NormalizePlanFileName(PlanFileName));
        }
        catch (Exception)
        {
            return string.Empty;
        }
    }
}
