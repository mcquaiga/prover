using System;
using System.Collections.Generic;

namespace Shared.Extensions
{
    public static class TypeHelpers
    {
        public static bool IsNumber(this object value)
        {
            return value is sbyte
                   || value is byte
                   || value is short
                   || value is ushort
                   || value is int
                   || value is uint
                   || value is long
                   || value is ulong
                   || value is float
                   || value is double
                   || value is decimal;
        }

        public static bool IsInteger(this object value)
        {
            return value is int
                   || value is uint
                   || value is ulong;
        }

        public static bool IsDecimal(this object value)
        {
            return value is decimal
                   || value is double
                   || value is float;
        }
    }

    public static class NumericHelpers
    {
        #region Methods

        public static bool IsBetween<T>(this T value, T lowerLimit, T upperLimit) where T : struct
        {
            return Comparer<T>.Default.Compare(value, lowerLimit) >= 0
                   && Comparer<T>.Default.Compare(value, upperLimit) <= 0;
        }

        public static bool IsBetween<T>(this T value, T absoluteLimit) where T : struct
        {
            if (!typeof(T).Equals(typeof(int))
                && !typeof(T).Equals(typeof(decimal))
                && !typeof(T).Equals(typeof(double)))
            {
                return false;
            }

            var valueDouble = double.Parse(value.ToString());
            var absDouble = double.Parse(absoluteLimit.ToString());
            var inverse = -1 * absDouble;

            if (inverse < absDouble)
                return valueDouble.IsBetween(inverse, absDouble);

            return valueDouble.IsBetween(absDouble, inverse);
        }

        public static bool IsInteger(this decimal value)
        {
            return (double)Math.Abs(value - (int)value) < double.Epsilon;
        }

        #endregion
    }
}