using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moral.Linq
{
    /// <summary>
    /// Linqの拡張メソッド群
    /// http://kamiya.hatenadiary.jp/entry/2015/02/05/140948
    /// </summary>
    public static class LinqExtention
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="func"></param>
        public static void ForEach<T>(this IEnumerable<T> src, Action<T, int> func)
        {
            foreach (var item in src.Select((x, i) => new { Value = x, Index = i }))
                func(item.Value, item.Index);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="src"></param>
        /// <param name="func"></param>
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> src, Action<T> func)
        {
            foreach (var item in src)
                func(item);

            return src;
        }

        public static IEnumerable<T> Insert<T>(this IEnumerable<T> src, int index, T insertItem)
        {
            foreach (var item in src.Select((x, i) => new { X = x, I = i }))
            {
                if (item.I == index) yield return insertItem;
                yield return item.X;
            }
        }
        public static IEnumerable<T> Insert<T>(this IEnumerable<T> src, int index, IEnumerable<T> insertSrc)
        {
            foreach (var item in src.Select((x, i) => new { X = x, I = i }))
            {
                if (item.I == index)
                    foreach (var insertItem in insertSrc)
                        yield return insertItem;
                yield return item.X;
            }
        }
    }
}
