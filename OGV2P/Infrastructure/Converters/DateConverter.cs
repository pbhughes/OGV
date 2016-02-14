using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.Converters
{
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
                if((DateTime)value == DateTime.MinValue )
                    return String.Empty;

            return ((DateTime)value).ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value.ToString();
            DateTime resultDateTime;
            return DateTime.TryParse(strValue, out resultDateTime) ? resultDateTime : value;
        }
    }
}