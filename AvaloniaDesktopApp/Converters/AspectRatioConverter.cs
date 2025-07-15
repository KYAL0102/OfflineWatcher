using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace AvaloniaDesktopApp.Converters;

public class AspectRatioConverter : IMultiValueConverter
{
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2 || !(values[0] is double) || !(values[1] is double))
        {
            return AvaloniaProperty.UnsetValue;
        }

        double width = (double)(values[0] ?? -1);
        double aspectRatio = (double)(values[1] ?? -1);
        return width / aspectRatio;
    }
}