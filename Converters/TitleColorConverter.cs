using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickDock.Converters
{
    public class TitleColorConverter : IValueConverter
    {
        private static readonly string[] Colors =
        {
            "#378ADD", "#1D9E75", "#D85A30",
            "#7F77DD", "#BA7517", "#D4537E",
            "#639922", "#185FA5", "#993556"
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string title && title.Length > 0)
            {
                int index = Math.Abs(title[0]) % Colors.Length;
                return new SolidColorBrush(
                    (Color)ColorConverter.ConvertFromString(Colors[index]));
            }
            return new SolidColorBrush(System.Windows.Media.Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}