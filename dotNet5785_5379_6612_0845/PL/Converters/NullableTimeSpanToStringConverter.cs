using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class NullableTimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
                return ts.ToString(@"hh\:mm\:ss");
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
