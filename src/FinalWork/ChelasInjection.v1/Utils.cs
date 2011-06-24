using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    static class Utils
    {
        public static T FirstWhere<T>(this IEnumerable<T> seq, Func<T, bool> predicate)
        {
            foreach (var t in seq)
            {
                if (predicate(t))
                    return t;
            }
            return default(T);
        }

        public static U ReturnFirstNotNull<T, U>(this IEnumerable<T> seq, Func<T, U> predicate)
        {
            foreach (var t in seq)
            {
                U u = predicate(t);
                if (u != null)
                {
                    return u;
                }
            }
            return default(U);
        }
    }
}
