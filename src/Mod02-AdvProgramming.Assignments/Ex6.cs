using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mod02_AdvProgramming.Assignments
{
    public static class Ex6
    {
        public static IEnumerable<T> MyWhere<T>(this IEnumerable<T> list, Func<T, bool> pred)
        {
            foreach (var l in list)
            {
                if (pred(l))
                    yield return l;
            }
        }

        public static IEnumerable<U> MySelect<T, U>(this IEnumerable<T> list, Func<T, U> proj)
        {
            foreach (var l in list)
            {
                yield return proj(l);
            }
        }

        public static IEnumerable<T> MyConcat<T>(this IEnumerable<T> list, IEnumerable<T> otherList)
        {
            foreach (var t in list)
            {
                yield return t;
            }
            foreach (var t in otherList)
            {
                yield return t;
            }
        }

        public static T MyFirst<T>(this IEnumerable<T> list, Func<T, bool> pred)
        {
            foreach (var t in list)
            {
                if (pred(t))
                {
                    return t;
                }
            }
            return default(T);
        }

        public static T MyLast<T>(this IEnumerable<T> list, Func<T, bool> pred)
        {
            T match = default(T);
            foreach (var t in list)
            {
                if (pred(t))
                {
                    match = t;
                }
            }
            return match;
        }
    }
}
