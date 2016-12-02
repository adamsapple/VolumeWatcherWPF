using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moral.Linq
{
    public static class LinqExtention
    {
        public static void ForEach<T>(this IEnumerable<T> src, Action<T, int> func)
        {
            foreach (var item in src.Select((x, i) => new { Value = x, Index = i }))
                func(item.Value, item.Index);
        }

        public static void ForEach<T>(this IEnumerable<T> src, Action<T> func)
        {
            foreach (var item in src)
                func(item);
        }
    }
}
