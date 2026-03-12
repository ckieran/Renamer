using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Renamer.UI.Plans;

public interface IPlanViewModel : INotifyPropertyChanged
{
    string PlanPath { get; set; }

    string StatusMessage { get; }

    string? ErrorMessage { get; }

    bool IsIdle { get; }

    bool IsLoading { get; }

    bool IsLoaded { get; }

    bool HasError { get; }

    ObservableCollection<PlanSummaryItem> SummaryItems { get; }

    ObservableCollection<PlanOperationItem> Operations { get; }

    Task BrowseAsync(CancellationToken cancellationToken = default);

    Task LoadAsync(CancellationToken cancellationToken = default);
}
