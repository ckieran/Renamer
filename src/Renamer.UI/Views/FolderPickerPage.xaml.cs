using System;
using System.IO;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Extensions.Logging;
using Renamer.Core.Services;
using CommunityToolkit.Maui.Storage;

namespace Renamer.UI.Views;

public partial class FolderPickerPage : ContentPage
{
    private string _selectedPath = string.Empty;
    private readonly IFileSystemService _fs;
    private readonly Microsoft.Extensions.Logging.ILogger<FolderPickerPage> _logger;

    public FolderPickerPage(IFileSystemService fs, Microsoft.Extensions.Logging.ILogger<FolderPickerPage> logger)
    {
        InitializeComponent();
        _fs = fs;
        _logger = logger;
    }

    private async void OnPickFolderClicked(object sender, EventArgs e)
    {
        // Ensure the platform file picker is invoked on the main thread to avoid deadlocks on some platforms (MacCatalyst)
        try
        {
            var folderPickerResult = await FolderPicker.Default.PickAsync();

            if (folderPickerResult.IsSuccessful)
            {
                var folderName = folderPickerResult.Folder.Name;
                var folderPath = folderPickerResult.Folder.Path;
                // Use the resolved path
                 _selectedPath = folderPath;
                _logger?.LogInformation("User selected folder: {FolderName} at {FolderPath}", folderName, folderPath);
            }
    

            SelectedPathLabel.Text = _selectedPath;
            GenerateBtn.IsEnabled = !string.IsNullOrEmpty(_selectedPath);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error while picking folder");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnGenerateClicked(object sender, EventArgs e)
    {
        try
        {
            // Use DI-provided filesystem service and navigate to plan view with query param
            await Shell.Current.GoToAsync($"//PlanView?root={Uri.EscapeDataString(_selectedPath)}");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}
