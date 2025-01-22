using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARWNI2S.Collections
{
    public static class EnumerableExtensions
    {
        public static TSource[] ToArray<TSource>(this IEnumerable<TSource> source, out int size)
        {
            if (source == null)
            {
                ArgumentNullException.ThrowIfNull(source);
            }

            var result = source.ToArray();
            size = result.Length;
            return result;
        }

    }
}
