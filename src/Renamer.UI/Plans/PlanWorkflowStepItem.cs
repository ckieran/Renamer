using System.ComponentModel;
using System.Runtime.CompilerServices;
using Renamer.UI.Resources.Strings;

namespace Renamer.UI.Plans;

public sealed class PlanWorkflowStepItem : INotifyPropertyChanged
{
    private PlanWorkflowStepStatus status;
    private bool isSelected;

    public PlanWorkflowStepItem(PlanWorkflowStep step, string title, string description)
    {
        Step = step;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        status = PlanWorkflowStepStatus.NeedsInfo;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public PlanWorkflowStep Step { get; }

    public string Title { get; }

    public string Description { get; }

    public PlanWorkflowStepStatus Status
    {
        get => status;
        set
        {
            if (SetProperty(ref status, value))
            {
                OnPropertyChanged(nameof(StatusText));
                OnPropertyChanged(nameof(StatusColor));
            }
        }
    }

    public string StatusText => Status switch
    {
        PlanWorkflowStepStatus.Done => AppStrings.StepStatusDone,
        PlanWorkflowStepStatus.Error => AppStrings.StepStatusError,
        _ => AppStrings.StepStatusNeedsInfo
    };

    public string StatusColor => Status switch
    {
        PlanWorkflowStepStatus.Done => "#166534",
        PlanWorkflowStepStatus.Error => "#9A3412",
        _ => "#5A6680"
    };

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }

    private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
