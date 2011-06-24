using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    partial class Injector
    {
        public class PerRequestInstanceManager : IActivationPlugIn
        {
            private Dictionary<TypeIndex, object> m_currentCallObjectList;
            public static PerRequestInstanceManager Singleton { get; private set; }

            static PerRequestInstanceManager()
            {
                Singleton = new PerRequestInstanceManager();
            }

            public PerRequestInstanceManager()
            {}

            #region IActivationPlugIn Members

            public void Init(IPlugInEvents eventController)
            {
                eventController.RequestStarted += new RequestStartedHandler(eventController_RequestStarted);
            }

            public object GetInstance(Injector.TypeIndex typeIndex)
            {
                // a type already created is always returned
                if (m_currentCallObjectList.ContainsKey(typeIndex))
                    return m_currentCallObjectList[typeIndex];

                return null;
            }

            public void NewInstance(Injector.TypeIndex typeIndex, object instance)
            {
                m_currentCallObjectList.Add(typeIndex, instance);
            }

            #endregion

            void eventController_RequestStarted(IPlugInEvents sender, TypeIndex t)
            {
                m_currentCallObjectList = new Dictionary<TypeIndex, object>();
            }

        }
    }
}
