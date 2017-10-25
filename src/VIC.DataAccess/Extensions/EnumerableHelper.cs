using System;
using System.Collections.Generic;
using System.Text;

namespace System.Linq
{
    public static class EnumerableHelper
    {
        public static IEnumerable<IEnumerable<T>> Page<T>(this IEnumerable<T> source, int pageSize)
        {
            var totalCount = (int)Math.Ceiling(source.Count() * 1.0 / pageSize);
            return Enumerable.Range(0, totalCount)
                .Select(i => source.Skip(pageSize * i).Take(pageSize));
        }
    }
}
