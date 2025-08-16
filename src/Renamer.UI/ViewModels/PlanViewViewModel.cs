using Microsoft.Maui.Controls;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Renamer.Core.Enums;
using Renamer.Core.Models;

namespace Renamer.UI.ViewModels;

public class PlanViewViewModel : INotifyPropertyChanged
{
    public ObservableCollection<FileOperation> Operations { get; } = new();
    public ICommand SimulateCommand { get; }

    private double _progress;
    public double Progress
    {
        get => _progress;
        set
        {
            if (_progress != value)
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
    }

    public PlanViewViewModel()
    {
        Operations.Add(new FileOperation { SourcePath="/a", DestinationPath="/b", OperationType = FileOperationType.FolderCreate });
        Operations.Add(new FileOperation { SourcePath="/b", DestinationPath="/c", OperationType = FileOperationType.FolderRename });
        Operations.Add(new FileOperation { SourcePath="/c", DestinationPath="/d", OperationType = FileOperationType.FileMove });
        Operations.Add(new FileOperation { SourcePath="/d", DestinationPath="/e", OperationType = FileOperationType.Error, ErrorMessage="Sample error" });
        SimulateCommand = new Command(async () => await SimulateAsync());
        UpdateProgress();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private async Task SimulateAsync()
    {
        foreach (var op in Operations)
        {
            await Task.Delay(200);
            op.Status = OperationStatus.Completed;
            UpdateProgress();
        }
    }

    private void UpdateProgress()
    {
        if (Operations.Count == 0)
            Progress = 0;
        else
            Progress = Operations.Count(o => o.Status == OperationStatus.Completed) / (double)Operations.Count;
    }
}
