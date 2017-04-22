using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prover.Shared.Extensions
{
    public static class ListExtensions
    {
        public static bool ScrambledEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();
            foreach (var s in list1)
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else
                    cnt.Add(s, 1);
            foreach (var s in list2)
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else
                    return false;
            return cnt.Values.All(c => c == 0);
        }
    }
}
