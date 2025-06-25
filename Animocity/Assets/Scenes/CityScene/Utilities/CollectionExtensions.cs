using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Utilities
{
    public static class CustomCollectionExtensions
    {
        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> data, Func<T, U> map)
        {
            foreach (T t in data)
            {
                yield return map(t);
            }
        }
        public static bool WhereAny<T>(this IEnumerable<T> data, Func<T, bool> predicate)
        {
            foreach(T t in data)
            {
                if(predicate(t)) return true;
            }
            return false;
        }

        public static bool WhereAll<T>(this IEnumerable<T> data, Func<T, bool> predicate)
        {
            foreach (T t in data)
            {
                if (!predicate(t)) return false;
            }
            return true;
        }
    }
}
