using System;

namespace ChelasInjection
{
    /// <summary>
    /// Interface to configure a type configuration binding
    /// </summary>
    /// <typeparam name="T">Type being configured</typeparam>
    public interface ITypeBinder<T>
    {
        IConstructorBinder<T> WithConstructor(params Type[] constructorArguments);

        ITypeBinder<T> WithNoArgumentsConstructor();

        IActivationBinder<T> WithActivation { get; set; }

        ITypeBinder<T> InitializeObjectWith(Action<T> initialization);

        void WhenArgumentHas<TAttribute>() where TAttribute : Attribute;

    }
}