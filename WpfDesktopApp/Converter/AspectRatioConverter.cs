using System.Globalization;
using System.Windows.Data;

namespace WpfDesktopApp.Converter;

public class AspectRatioConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 2 || !(values[0] is double) || !(values[1] is double))
        {
            return Binding.DoNothing;
        }

        double width = (double)values[0];
        double aspectRatio = (double)values[1];
        return width / aspectRatio;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}