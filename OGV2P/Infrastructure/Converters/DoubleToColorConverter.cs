﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Infrastructure.Converters
{
    public class DoubleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Diagnostics.Debug.Write(string.Format("Vu Meter Reading: {0}", value.ToString()));
            if ((double)value <= 11000)
                return Colors.Green;

            if ((double)value > 11000 && (double)value <= 22000)
                return Colors.Yellow;

            return Colors.Red;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}