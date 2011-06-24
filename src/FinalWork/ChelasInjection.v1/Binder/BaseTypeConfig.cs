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
                ConstructorType = ConstructorTypeConfig.NoValue;
            }

            public enum ConstructorTypeConfig
            {
                NoValue,
                NoArguments,
                Values,
                Action
            }
            public ConstructorTypeConfig ConstructorType { get; protected set; }
            public Type[] ConstructorArguments { get; protected set; }
            public Func<object> ConstructorValues { get; protected set; }
            public Action<object> ConstructorAction { get; protected set; }
            public Type AttributeType { get; protected set; }

            protected enum ActivationType
            {
                NoValue,
                Singleton,
                PerRequest
            }
            protected ActivationType m_activationType = ActivationType.NoValue;

            public bool ActivationSingleton
            {
                get { return m_activationType == ActivationType.Singleton; }
            }
            public bool ActivationPerRequest
            {
                get { return m_activationType == ActivationType.PerRequest; }
            }


            public Func<object> Constructor { get; set; }
        }
    }
}
