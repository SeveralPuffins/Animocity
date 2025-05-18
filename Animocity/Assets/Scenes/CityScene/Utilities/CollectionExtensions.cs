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
    }
}
