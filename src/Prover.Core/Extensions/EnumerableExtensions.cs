using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static List<T> AddEmptyItem<T>(this IEnumerable<T> items, Func<T> emptyItemFactory)
        {
            var result = items.ToList();
            result.Add(emptyItemFactory.Invoke());
            return result;
        }
    }
}
