using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class BoolToSimulatorTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? "Stop Simulator" : "Start Simulator";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
