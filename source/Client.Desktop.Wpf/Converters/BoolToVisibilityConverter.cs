using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Client.Desktop.Wpf.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public BoolToVisibilityConverter()
            : this(Visibility.Visible, Visibility.Collapsed)
        {
        }

        public BoolToVisibilityConverter(Visibility trueValue, Visibility falseValue)
        {
            // set defaults
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value == null)
                return FalseValue;

            if (!(value is bool))
                return TrueValue;

            return (bool) value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }

    //public class BoolToVisibilityBinding : IBindingTypeConverter
    //{
    //    public Visibility TrueValue { get; set; } = Visibility.Visible;
    //    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

    //    public int GetAffinityForObjects(Type fromType, Type toType) => 2;

    //    public bool TryConvert(object @from, Type toType, object conversionHint, out object result)
    //    {

    //    }
    //}
}