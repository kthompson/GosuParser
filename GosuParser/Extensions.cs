using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.Remoting.Messaging;

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