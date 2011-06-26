using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    partial class Binder
    {
        /// <summary>
        /// Class that implements all the methods of the type configuration interfaces, by setting the configuration properties in
        /// the BaseTypeConfig parent class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        class TypeConfig<T> : BaseTypeConfig, ITypeBinder<T>, IConstructorBinder<T>, IActivationBinder<T>
        {
            public TypeConfig(Type t)
                : base(t)
            { }

            #region ITypeBinder<T> Members

            public IConstructorBinder<T> WithConstructor(params Type[] constructorArguments)
            {
                ConstructorType = ConstructorTypeConfig.Values;
                ConstructorArguments = constructorArguments;
                return this;
            }

            public ITypeBinder<T> WithNoArgumentsConstructor()
            {
                ConstructorType = ConstructorTypeConfig.NoArguments;
                return this;
            }

            public IActivationBinder<T> WithActivation
            {
                get
                {
                    return this;
                }
                set
                { }
            }

            public ITypeBinder<T> InitializeObjectWith(Action<T> initialization)
            {
                ConstructorType = ConstructorTypeConfig.Action;
                ConstructorAction = o => initialization((T)o);
                return this;
            }

            public void WhenArgumentHas<TAttribute>() where TAttribute : Attribute
            {
                AttributeType = typeof(TAttribute);
            }

            #endregion

            #region IConstructorBinder<T> Members

            public ITypeBinder<T> WithValues(Func<object> values)
            {
                ConstructorValues = values;
                return this;
            }

            #endregion

            #region IActivationBinder<T> Members

            public IActivationPlugIn ActivationPlugIn
            {
                set
                {
                    ActivationObject = value;
                }
            }

            public ITypeBinder<T> GetCurrentTypeBinder
            {
                get
                {
                    return this;
                }
            }


            #endregion
        }
    }
}
