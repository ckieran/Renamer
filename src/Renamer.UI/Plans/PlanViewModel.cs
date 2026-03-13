using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Renamer.Core.Contracts;
using Renamer.Core.Serialization;
using Microsoft.Extensions.Logging;

namespace Renamer.UI.Plans;

public sealed class PlanViewModel : IPlanViewModel
{
    private readonly IPlanFilePicker planFilePicker;
    private readonly IPlanSerializer planSerializer;
    private readonly ILogger<PlanViewModel> logger;

    private string planPath = string.Empty;
    private string statusMessage = "Select a rename-plan.json file to preview planned operations.";
    private string? errorMessage;
    private PlanViewState state = PlanViewState.Idle;

    public PlanViewModel(
        IPlanSerializer planSerializer,
        IPlanFilePicker planFilePicker,
        ILogger<PlanViewModel> logger)
    {
        this.planSerializer = planSerializer ?? throw new ArgumentNullException(nameof(planSerializer));
        this.planFilePicker = planFilePicker ?? throw new ArgumentNullException(nameof(planFilePicker));
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

    public ObservableCollection<PlanSummaryItem> SummaryItems { get; } = [];

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
        SummaryItems.Clear();
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
        SummaryItems.Add(new PlanSummaryItem("Plan ID", plan.PlanId));
        SummaryItems.Add(new PlanSummaryItem("Root Path", plan.RootPath));
        SummaryItems.Add(new PlanSummaryItem("Created At UTC", plan.CreatedAtUtc));
        SummaryItems.Add(new PlanSummaryItem("Operation Count", plan.Summary.OperationCount.ToString()));
        SummaryItems.Add(new PlanSummaryItem("Warnings", plan.Summary.Warnings.ToString()));

        foreach (var operation in plan.Operations)
        {
            Operations.Add(new PlanOperationItem(
                operation.OpId,
                operation.SourcePath,
                operation.PlannedDestinationPath,
                $"{operation.Reason.StartDate} to {operation.Reason.EndDate}",
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
        SummaryItems.Clear();
        Operations.Clear();
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
}
