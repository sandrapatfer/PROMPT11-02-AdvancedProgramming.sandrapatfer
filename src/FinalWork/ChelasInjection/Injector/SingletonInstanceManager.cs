using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    partial class Injector
    {
        /// <summary>
        /// Plug In for handling the instances created in singleton mode
        /// </summary>
        public class SingletonInstanceManager : IActivationPlugIn
        {
            private Dictionary<TypeIndex, object> m_singletonList = new Dictionary<TypeIndex, object>();

            static SingletonInstanceManager()
            {
                Singleton = new SingletonInstanceManager();
            }

            private SingletonInstanceManager()
            {}

            public static SingletonInstanceManager Singleton { get; private set; }

            #region IActivationPlugIn Members

            public void Init(IPlugInEvents eventController)
            {
            }

            public object GetInstance(Injector.TypeIndex typeIndex)
            {
                // a type configured as singleton always returns the same object, independently of the configuration
                if (m_singletonList.ContainsKey(typeIndex))
                {
                    return m_singletonList[typeIndex];
                }
                else
                {
                    return null;
                }
            }

            public void NewInstance(Injector.TypeIndex typeIndex, object instance)
            {
                m_singletonList.Add(typeIndex, instance);
            }

            #endregion
        }
    }
}
