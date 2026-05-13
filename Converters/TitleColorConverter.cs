using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace QuickDock.Converters
{
    public class TitleColorConverter : IValueConverter
    {
        private static readonly Dictionary<string, string> SiteColors = new(StringComparer.OrdinalIgnoreCase)
        {
            { "github",   "#1A1A1A" },
            { "notion",   "#1A1A1A" },
            { "figma",    "#6B5ECD" },
            { "youtube",  "#E53935" },
            { "chatgpt",  "#1EA672" },
            { "discord",  "#5865F2" },
            { "supabase", "#3ECF8E" },
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string title && title.Length > 0)
            {
                foreach (var (keyword, hex) in SiteColors)
                {
                    if (title.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex));
                }
            }
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#888888"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}