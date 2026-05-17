using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickDock.Converters
{
    public class StringToBrushConverter : IValueConverter
    {
        private static readonly Dictionary<string, SolidColorBrush> _cache = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string hex || hex.Length == 0)
                return Brushes.Gray;

            if (_cache.TryGetValue(hex, out var brush))
                return brush;

            try
            {
                brush = (SolidColorBrush)new BrushConverter().ConvertFrom(hex)!;
                brush.Freeze();
            }
            catch
            {
                brush = Brushes.Gray;
            }
            _cache[hex] = brush;
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotSupportedException();
    }
}
