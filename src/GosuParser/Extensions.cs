using System.Collections.Generic;

namespace GosuParser
{
    public static class Extensions
    {
        public static IEnumerable<T> Cons<T>(this T item1, IEnumerable<T> collection)
        {
            yield return item1;

            foreach (var item in collection)
                yield return item;
        }
    }
}