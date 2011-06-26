using System;

namespace ChelasInjection
{
    /// <summary>
    /// Interface to configure the constructor values to be set, in a type configuration binding
    /// </summary>
    /// <typeparam name="T">Type being configured</typeparam>
    public interface IConstructorBinder<T>
    {
        ITypeBinder<T> WithValues(Func<object> values);
    }
}