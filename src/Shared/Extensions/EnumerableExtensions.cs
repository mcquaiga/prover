using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Extensions
{
    public static class EnumerableExtensions
    {
        #region Public Methods

        public static List<T> AddEmptyItem<T>(this IEnumerable<T> items, Func<T> emptyItemFactory)
        {
            var result = items.ToList();
            result.Add(emptyItemFactory.Invoke());
            return result;
        }

        public static HashSet<T> ToHashSet<T>(
            this IEnumerable<T> source,
            IEqualityComparer<T> comparer = null)
        {
            return new HashSet<T>(source, comparer);
        }

        #endregion
    }
}