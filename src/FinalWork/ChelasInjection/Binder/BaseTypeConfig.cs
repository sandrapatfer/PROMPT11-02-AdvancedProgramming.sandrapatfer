using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    partial class Binder
    {
        internal class BaseTypeConfig
        {
            public Type TargetType { get; private set; }

            public BaseTypeConfig(Type type)
            {
                TargetType = type;
            }

            protected Type[] m_constructorArguments;
            protected Func<object> m_constructorValues;
            protected Action<object> m_constructorCode;
            protected bool m_withNoArgumentsContructor = false;

            protected enum ActivationType
            {
                Singleton,
                PerRequest
            }
            protected ActivationType m_activationType = ActivationType.PerRequest;

            public bool ActivationSingleton
            {
                get { return m_activationType == ActivationType.Singleton; }
            }
            public bool ActivationPerRequest
            {
                get { return m_activationType == ActivationType.PerRequest; }
            }
    
        }
    }
}
