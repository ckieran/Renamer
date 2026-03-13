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

    string RootPath { get; }

    string CreatedAtDisplay { get; }

    string OperationCountText { get; }

    string WarningCountText { get; }

    bool CanOpenRootPath { get; }

    System.Collections.ObjectModel.ObservableCollection<PlanOperationItem> Operations { get; }

    Task BrowseAsync(CancellationToken cancellationToken = default);

    Task LoadAsync(CancellationToken cancellationToken = default);

    Task OpenRootPathAsync(CancellationToken cancellationToken = default);
}
