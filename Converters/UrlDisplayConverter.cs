using System;
using System.Globalization;
using System.Windows.Data;

namespace QuickDock.Converters
{
    public class UrlDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not string url || string.IsNullOrWhiteSpace(url))
                return string.Empty;

            if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                url = url[8..];
            else if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                url = url[7..];

            if (url.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
                url = url[4..];

            return url;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
