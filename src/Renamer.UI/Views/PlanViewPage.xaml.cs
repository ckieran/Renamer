using Renamer.Core.Models;
using Renamer.Core.Enums;
using System.Linq;
using Microsoft.Maui.Controls;
using Renamer.UI.ViewModels;

namespace Renamer.UI.Views;

public partial class PlanViewPage : ContentPage
{
    public PlanViewPage()
    {
        InitializeComponent();
        BindingContext = new PlanViewViewModel();
    }

    private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Renamer.Core.Models.FileOperation op && op.OperationType == Renamer.Core.Enums.FileOperationType.Error)
        {
            await DisplayAlert("Error", op.ErrorMessage, "OK");
        }
    }
}
