using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Newtonsoft.Json;

namespace Prover.GUI.Common.Converters
{
    public class NullToVisibilityConverter : IValueConverter
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
            return value == null ? NullValue : NotNullValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}