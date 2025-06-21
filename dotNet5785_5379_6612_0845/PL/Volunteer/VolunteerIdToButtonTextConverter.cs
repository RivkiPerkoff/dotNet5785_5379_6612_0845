
using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Volunteer
{
    public class VolunteerIdToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int id && id > 0)
                return "עדכן מתנדב";
            return "הוסף מתנדב";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
