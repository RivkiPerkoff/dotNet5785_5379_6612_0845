using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class MaxFinishTimeToTimeLeftConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime maxFinishTime)
            {
                TimeSpan timeLeft = maxFinishTime - DateTime.Now;
                return timeLeft > TimeSpan.Zero ? timeLeft.ToString(@"hh\:mm\:ss") : "00:00:00";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
