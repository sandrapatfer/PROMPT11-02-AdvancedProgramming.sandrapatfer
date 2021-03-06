﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    public static class Utils
    {
        public static ITypeBinder<T> Singleton<T>(this IActivationBinder<T> typeBinder)
        {
            typeBinder.ActivationPlugIn = Injector.SingletonInstanceManager.Singleton;
            return typeBinder.GetCurrentTypeBinder;
        }

        public static ITypeBinder<T> PerRequest<T>(this IActivationBinder<T> typeBinder)
        {
            typeBinder.ActivationPlugIn = Injector.PerRequestInstanceManager.Singleton;
            return typeBinder.GetCurrentTypeBinder;
        }

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
