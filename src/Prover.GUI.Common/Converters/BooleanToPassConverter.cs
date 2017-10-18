using System;
using System.Globalization;
using System.Windows.Data;

namespace Prover.GUI.Common.Converters
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
            if (value is bool && (bool) value)
                return TrueValue;

            return FalseValue;
        }

        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string) value == "P")
                return true;

            return false;
        }
    }
}