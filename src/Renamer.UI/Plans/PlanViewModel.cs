using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Renamer.Core.Contracts;
using Renamer.Core.Execution;
using Renamer.Core.Serialization;

namespace Renamer.UI.Plans;

public sealed class PlanViewModel : IPlanViewModel
{
    private readonly IPlanFilePicker planFilePicker;
    private readonly IPlanSerializer planSerializer;
    private readonly IRootPathOpener rootPathOpener;
    private readonly IApplyEngine applyEngine;
    private readonly ILogger<PlanViewModel> logger;

    private RenamePlan? loadedPlan;
    private string planPath = string.Empty;
    private string statusMessage = "Select a rename-plan.json file to preview planned operations.";
    private string? errorMessage;
    private string rootPath = string.Empty;
    private string createdAtDisplay = "No plan loaded";
    private string operationCountText = "0";
    private string warningCountText = "0";
    private string applyStatusMessage = "Load a plan preview to enable apply.";
    private string? applyErrorTitle;
    private string? applyErrorMessage;
    private string applyOutcomeText = "Not run";
    private string applyStartedAtDisplay = "Not run";
    private string applyFinishedAtDisplay = "Not run";
    private string applySuccessCountText = "0";
    private string applyFailedCountText = "0";
    private string applySkippedCountText = "0";
    private string applyDriftedCountText = "0";
    private PlanViewState state = PlanViewState.Idle;
    private bool isApplying;
    private bool hasApplyReport;

    public PlanViewModel(
        IPlanSerializer planSerializer,
        IPlanFilePicker planFilePicker,
        IRootPathOpener rootPathOpener,
        IApplyEngine applyEngine,
        ILogger<PlanViewModel> logger)
    {
        this.planSerializer = planSerializer ?? throw new ArgumentNullException(nameof(planSerializer));
        this.planFilePicker = planFilePicker ?? throw new ArgumentNullException(nameof(planFilePicker));
        this.rootPathOpener = rootPathOpener ?? throw new ArgumentNullException(nameof(rootPathOpener));
        this.applyEngine = applyEngine ?? throw new ArgumentNullException(nameof(applyEngine));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

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

    public async Task BrowseAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Plan browse requested.");
        StatusMessage = "Opening plan file picker...";

        try
        {
            var selectedPath = await planFilePicker.PickPlanPathAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(selectedPath))
            {
                logger.LogInformation("Plan selection canceled.");
                StatusMessage = "Plan selection canceled.";
                return;
            }

            PlanPath = selectedPath;
            ErrorMessage = null;
            StatusMessage = $"Selected plan artifact: {Path.GetFileName(selectedPath)}";
            logger.LogInformation("Plan artifact selected: {PlanPath}", selectedPath);
        }
        catch (Exception ex) when (ex is InvalidOperationException or NotSupportedException or IOException)
        {
            logger.LogError(ex, "Plan selection failed.");
            SetErrorState($"Unable to select a plan artifact: {ex.Message}");
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
            StatusMessage = "Opened root folder.";
        }
        catch (Exception ex) when (ex is IOException or InvalidOperationException or NotSupportedException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Unable to open plan root path {RootPath}.", RootPath);
            SetErrorState($"Unable to open root folder: {ex.Message}");
        }
    }

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(PlanPath))
        {
            logger.LogWarning("Plan load requested without a selected path.");
            SetErrorState("Select a plan artifact path to load.");
            return;
        }

        logger.LogInformation("Loading plan preview from {PlanPath}.", PlanPath);
        SetState(PlanViewState.Loading);
        ErrorMessage = null;
        StatusMessage = "Loading plan preview...";
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
            SetErrorState($"Unable to load plan artifact: {ex.Message}");
        }
    }

    public async Task ApplyAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(PlanPath) || loadedPlan is null || !IsLoaded)
        {
            logger.LogWarning("Apply requested without a loaded plan.");
            SetApplyError("Apply validation failed", "Select and load a valid plan artifact before apply.");
            return;
        }

        logger.LogInformation("Applying plan artifact {PlanPath}.", PlanPath);
        IsApplying = true;
        ApplyErrorTitle = null;
        ApplyErrorMessage = null;
        ApplyStatusMessage = "Applying rename plan...";
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
                SetApplyError(
                    "Apply stopped after conflict retry limit",
                    "A destination conflict could not be resolved after 10 suffix retries.");
                ApplyStatusMessage = "Apply stopped before the full plan completed.";
                return;
            }

            ApplyStatusMessage = $"Apply completed with {report.Summary.Success} success, {report.Summary.Skipped} skipped, {report.Summary.Failed} failed.";
        }
        catch (Exception ex) when (ex is InvalidDataException or NotSupportedException or System.Text.Json.JsonException)
        {
            logger.LogError(ex, "Plan schema validation failed during apply for {PlanPath}.", PlanPath);
            SetApplyError("Plan artifact is invalid", $"Unable to apply the selected plan artifact: {ex.Message}");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogError(ex, "Apply failed due to file system access for {PlanPath}.", PlanPath);
            SetApplyError("Apply failed due to file system error", $"Unable to complete apply: {ex.Message}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected apply failure for {PlanPath}.", PlanPath);
            SetApplyError("Apply failed unexpectedly", ex.Message);
        }
        finally
        {
            IsApplying = false;
        }
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
        StatusMessage = $"Loaded {plan.Operations.Count} planned operation(s).";
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
    }

    private void SetErrorState(string message)
    {
        SetState(PlanViewState.Error);
        ErrorMessage = message;
        StatusMessage = "Plan preview unavailable.";
        ClearLoadedData();
        Operations.Clear();
        loadedPlan = null;
        ResetApplyState();
    }

    private void ClearLoadedData()
    {
        RootPath = string.Empty;
        CreatedAtDisplay = "No plan loaded";
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
            ? "Plan path updated. Load preview to refresh."
            : "Select a rename-plan.json file to preview planned operations.";
    }

    private void ResetApplyState()
    {
        ApplyErrorTitle = null;
        ApplyErrorMessage = null;
        ApplyStatusMessage = loadedPlan is null
            ? "Load a plan preview to enable apply."
            : "Ready to apply the loaded plan.";
        ApplyResults.Clear();
        HasApplyReport = false;
        IsApplying = false;
        ResetApplySummary();
    }

    private void ResetApplySummary()
    {
        ApplyOutcomeText = "Not run";
        ApplyStartedAtDisplay = "Not run";
        ApplyFinishedAtDisplay = "Not run";
        ApplySuccessCountText = "0";
        ApplyFailedCountText = "0";
        ApplySkippedCountText = "0";
        ApplyDriftedCountText = "0";
    }

    private void SetApplyError(string title, string message)
    {
        ApplyErrorTitle = title;
        ApplyErrorMessage = message;
        ApplyStatusMessage = "Apply unavailable.";
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
}
