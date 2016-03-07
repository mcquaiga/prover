using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Extensions
{
    public static class Helpers
    {
        public static bool IsBetween<T>(this T value, T lowerLimit, T upperLimit)
        {
            return Comparer<T>.Default.Compare(value, lowerLimit) >= 0
                    && Comparer<T>.Default.Compare(value, upperLimit) <= 0;
        }

        public static bool IsBetween<T>(this T value, T absoluteLimit)
        {
            //Make the limit 
            var inverse = -(dynamic)absoluteLimit;
            
            if (inverse < absoluteLimit)
                return value.IsBetween((T)inverse, absoluteLimit);
            else
                return value.IsBetween(absoluteLimit, (T)inverse);
        }

        public static bool IsInteger(this decimal value)
        {
            return (double)Math.Abs(value - (int)value) < double.Epsilon;
        }
    }
}
