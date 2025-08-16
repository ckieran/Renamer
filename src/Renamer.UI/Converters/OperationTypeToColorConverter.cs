using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Renamer.Core.Enums;

namespace Renamer.UI.Converters;

public class OperationTypeToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FileOperationType type)
        {
            return type switch
            {
                FileOperationType.FolderCreate => Colors.Blue,
                FileOperationType.FolderRename => Colors.Green,
                FileOperationType.FileMove => Colors.Yellow,
                FileOperationType.Error => Colors.Red,
                _ => Colors.Transparent
            };
        }
        return Colors.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
