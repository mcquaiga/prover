using System;
using System.Globalization;
using System.Windows.Data;

namespace Prover.GUI.Converters
{
    public class BooleanToPassConverter : IValueConverter
    {
        public BooleanToPassConverter()
        {
            TrueValue = "Pass";
            FalseValue = "Fail";
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return TrueValue;

            return FalseValue;
        }

        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && s == TrueValue)
                return true;

            return false;
        }
    }
}