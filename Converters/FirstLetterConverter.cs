using System;
using System.Globalization;
using System.Windows.Data;

namespace QuickDock.Converters
{
    public class FirstLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string title && title.Length > 0)
                return title[0].ToString().ToUpper();

            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}