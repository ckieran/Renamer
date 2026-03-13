using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Renamer.Core.Contracts;
using Renamer.Core.Serialization;

namespace Renamer.UI.Plans;

public sealed class PlanViewModel : IPlanViewModel
{
    private readonly IPlanFilePicker planFilePicker;
    private readonly IPlanSerializer planSerializer;
    private readonly IRootPathOpener rootPathOpener;
    private readonly ILogger<PlanViewModel> logger;

    private string planPath = string.Empty;
    private string statusMessage = "Select a rename-plan.json file to preview planned operations.";
    private string? errorMessage;
    private string rootPath = string.Empty;
    private string createdAtDisplay = "No plan loaded";
    private string operationCountText = "0";
    private string warningCountText = "0";
    private PlanViewState state = PlanViewState.Idle;

    public PlanViewModel(
        IPlanSerializer planSerializer,
        IPlanFilePicker planFilePicker,
        IRootPathOpener rootPathOpener,
        ILogger<PlanViewModel> logger)
    {
        this.planSerializer = planSerializer ?? throw new ArgumentNullException(nameof(planSerializer));
        this.planFilePicker = planFilePicker ?? throw new ArgumentNullException(nameof(planFilePicker));
        this.rootPathOpener = rootPathOpener ?? throw new ArgumentNullException(nameof(rootPathOpener));
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

    public ObservableCollection<PlanOperationItem> Operations { get; } = [];

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

        try
        {
            var plan = await Task.Run(() => planSerializer.Read(PlanPath), cancellationToken);
            PopulateLoadedState(plan);
            logger.LogInformation("Loaded plan preview with {OperationCount} operations.", plan.Operations.Count);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or NotSupportedException)
        {
            logger.LogError(ex, "Unable to load plan preview from {PlanPath}.", PlanPath);
            SetErrorState($"Unable to load plan artifact: {ex.Message}");
        }
    }

    private void PopulateLoadedState(RenamePlan plan)
    {
        RootPath = plan.RootPath;
        CreatedAtDisplay = FormatCreatedAt(plan.CreatedAtUtc);
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

    private void SetErrorState(string message)
    {
        SetState(PlanViewState.Error);
        ErrorMessage = message;
        StatusMessage = "Plan preview unavailable.";
        ClearLoadedData();
        Operations.Clear();
    }

    private void ClearLoadedData()
    {
        RootPath = string.Empty;
        CreatedAtDisplay = "No plan loaded";
        OperationCountText = "0";
        WarningCountText = "0";
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

    private static string FormatCreatedAt(string createdAtUtc)
    {
        if (!DateTimeOffset.TryParse(
                createdAtUtc,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var createdAt))
        {
            return createdAtUtc;
        }

        return createdAt.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture);
    }

    private static string FormatDateRange(string startDate, string endDate) =>
        startDate == endDate ? startDate : $"{startDate} to {endDate}";
}
