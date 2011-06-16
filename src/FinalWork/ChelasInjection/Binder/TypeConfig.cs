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

            #endregion

            #region IConstructorBinder<T> Members

            public ITypeBinder<T> WithValues(Func<object> values)
            {
                ConstructorValues = values;
                return this;
            }

            #endregion

            #region IActivationBinder<T> Members

            public ITypeBinder<T> PerRequest()
            {
                //TODO perguntar se esta activacao é mesmo suposto ser igual
                m_activationType = ActivationType.PerRequest;
                return this;
            }

            public ITypeBinder<T> Singleton()
            {
                m_activationType = ActivationType.Singleton;
                return this;
            }

            #endregion
        }
    }
}
