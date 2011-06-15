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
            
            // TODO pode ser um enum?
            protected bool m_withNoArgumentsContructor = false;
            protected bool m_withSingletonActivation = false;
            protected bool m_withPerRequestActivation = false;

        }
    }
}
