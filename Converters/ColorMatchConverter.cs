using System.Globalization;
using System.Windows.Data;

namespace QuickDock.Converters
{
    public class ColorMatchConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return false;
            return values[0] is string a && values[1] is string b && a == b;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
