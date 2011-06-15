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

        public static IEnumerable<T> MyTake<T>(this IEnumerable<T> list, int nElems)
        {
            int nReturned = 0;
            foreach (var t in list)
            {
                if (nReturned < nElems)
                {
                    nReturned++;
                    yield return t;
                }
            }
        }
        
        public static IEnumerable<R> MyZip<T, U, R>(this IEnumerable<T> list, IEnumerable<U> other, Func<T, U, R> conv)
        {
            var enumT = list.GetEnumerator();
            var enumU = other.GetEnumerator();
            
            while(enumT.MoveNext() && enumU.MoveNext())
            {
                yield return conv(enumT.Current, enumU.Current);
            }
        }

        public static T MyAggregate<T>(this IEnumerable<T> list, Func<T, T, T> aggregator)
        {
            T total = default(T);
            foreach (var t in list)
            {
                total = aggregator(total, t);
            }
            return total;
        }

        public static IEnumerable<R> MyJoin<T, U, K, R>(this IEnumerable<T> list, IEnumerable<U> other, Func<T, K> keyT, Func<U, K> keyU, Func<T, U, R> conv)
        {
            var enumT = list.GetEnumerator();
            while (enumT.MoveNext())
            {
                var enumU = other.GetEnumerator();
                while (enumU.MoveNext())
                {
                    if (keyT(enumT.Current).GetHashCode() == keyU(enumU.Current).GetHashCode())
                        yield return conv(enumT.Current, enumU.Current);
                }
            }
        }
    }
}
