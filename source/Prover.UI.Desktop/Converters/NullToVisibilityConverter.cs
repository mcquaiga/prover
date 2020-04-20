using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Prover.UI.Desktop.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public sealed class NullToVisibilityConverter : IValueConverter
    {
        public NullToVisibilityConverter()
        {
            NullValue = Visibility.Collapsed;
            NotNullValue = Visibility.Visible;
        }

        public Visibility NullValue { get; set; }
        public Visibility NotNullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                value = string.IsNullOrEmpty((string) value) ? null : value;

            return value == null ? NullValue : NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(object), typeof(bool))]
    public sealed class NullToBoolConverter : IValueConverter
    {
        public NullToBoolConverter()
        {
            NullValue = false;
            NotNullValue = true;
        }

        public bool NullValue { get; set; }
        public bool NotNullValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                value = string.IsNullOrEmpty((string) value) ? null : value;

            return value == null ? NullValue : NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}