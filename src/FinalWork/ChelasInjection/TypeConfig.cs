using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    partial class Binder
    {
        class TypeConfig<T> : BaseTypeConfig, ITypeBinder<T>, IConstructorBinder<T>, IActivationBinder<T>
        {
            public TypeConfig(Type t)
                : base(t)
            { }

            #region ITypeBinder<T> Members

            public IConstructorBinder<T> WithConstructor(params Type[] constructorArguments)
            {
                m_constructorArguments = constructorArguments;
                return this;
            }

            public ITypeBinder<T> WithNoArgumentsConstructor()
            {
                m_withNoArgumentsContructor = true;
                return this;
            }

            public ITypeBinder<T> WithSingletonActivation()
            {
                m_withSingletonActivation = true;
                return this;
            }

            public ITypeBinder<T> WithPerRequestActivation()
            {
                m_withPerRequestActivation = true;
                return this;
            }

            public IActivationBinder<T> WithActivation
            {
                get
                {
                    return this;
                }
            }

            public ITypeBinder<T> InitializeObjectWith(Action<T> initialization)
            {
                m_constructorCode = o => initialization((T)o);
                return this;
            }

            #endregion

            #region IConstructorBinder<T> Members

            public ITypeBinder<T> WithValues(Func<object> values)
            {
                m_constructorValues = values;
                return this;
            }

            #endregion

            #region IActivationBinder<T> Members

            public ITypeBinder<T> PerRequest()
            {
                throw new NotImplementedException();
            }

            public ITypeBinder<T> Singleton()
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}
