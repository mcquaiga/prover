using System;
using System.Linq;

namespace Prover.Core.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        ///     Get substring of specified number of characters on the right.
        /// </summary>
        public static string Right(this string value, int length)
        {
            return value.Substring(value.Length - length);
        }

        public static T ParseEnum<T>(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return default(T);
            //throw new ArgumentNullException(nameof(value));
            var t = typeof(T);
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                t = t.GetGenericArguments().First();
            }

            return (T) Enum.Parse(t, value, true);
        }
    }
}