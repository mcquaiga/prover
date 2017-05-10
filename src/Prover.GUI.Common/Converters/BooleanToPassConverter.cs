using System;
using System.Globalization;
using System.Windows.Data;

namespace Prover.GUI.Common.Converters
{
    public class BooleanToPassConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool) value)
                return "P";

            return "F";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string) value == "P")
                return true;

            return false;
        }
    }
}