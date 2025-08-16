using Renamer.UI.Views;
using System.ComponentModel;
using Microsoft.Maui.Controls;
using System.IO;
using System.Windows.Input;
using Microsoft.Maui.Storage;

namespace Renamer.UI.ViewModels;

public class FolderPickerViewModel : INotifyPropertyChanged
{
    private string? _selectedFolder;
    public string? SelectedFolder
    {
        get => _selectedFolder;
        set
        {
            if (_selectedFolder != value)
            {
                _selectedFolder = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedFolder)));
            }
        }
    }

    public ICommand BrowseCommand { get; }
    public ICommand NextCommand { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public FolderPickerViewModel()
    {
        BrowseCommand = new Command(async () => await BrowseAsync());
        NextCommand = new Command(async () =>
        {
            if (string.IsNullOrWhiteSpace(SelectedFolder))
            {
                await Shell.Current.DisplayAlert("Error", "Please select a folder", "OK");
                return;
            }
            await Shell.Current.GoToAsync(nameof(PlanViewPage));
        });
    }

    private async Task BrowseAsync()
    {
        // Placeholder folder picker using file picker to get folder name
        var result = await FilePicker.Default.PickAsync();
        if (result != null)
            SelectedFolder = Path.GetDirectoryName(result.FullPath);
    }
}
