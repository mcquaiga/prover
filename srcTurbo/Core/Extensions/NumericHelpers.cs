using System;
using System.Collections.Generic;

namespace Core.Extensions
{
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