using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters
{
    public class ListCountToVisibilityConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool hasItems = value is ICollection collection && collection.Count > 0;

            if (parameter?.ToString() == "invert" || Invert)
                hasItems = !hasItems;

            return hasItems ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
