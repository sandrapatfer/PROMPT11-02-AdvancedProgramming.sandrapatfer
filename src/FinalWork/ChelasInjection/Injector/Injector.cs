using System;
using System.Collections.Generic;

namespace ChelasInjection
{
    public partial class Injector : IPlugInController, IPlugInEvents
    {
        private Binder _myBinder;
        private InstanceManager _myInstanceManager;

        public Injector(Binder myBinder)
        {
            _myBinder = myBinder;
            _myBinder.Configure(this);
            _myInstanceManager = new InstanceManager(_myBinder);
            m_plugInList.ForEach(p => p.Init(this));
        }

        public T GetInstance<T>()
        {
            return GetInstance<T>(new TypeIndex() { Type = typeof(T) });
        }

        public T GetInstance<T, TA>() where TA : Attribute
        {
            return GetInstance<T>(new TypeIndex() { Type = typeof(T), Attribute = typeof(TA) });
        }

        private T GetInstance<T>(TypeIndex tIndex)
        {
            if (RequestStarted != null)
                RequestStarted(this, tIndex);
            var obj = _myInstanceManager.GetInstance(tIndex);
            if (RequestStopped != null)
                RequestStopped(this, tIndex);
            return (T)obj;
        }

        public T GetInstance<T>(string name)
        {
            throw new NotImplementedException();
        }


        #region IPlugInController Members

        private List<IActivationPlugIn> m_plugInList = new List<IActivationPlugIn>();

        public void NewPlugIn(IActivationPlugIn plugIn)
        {
            if (plugIn != null)
            {
                if (!m_plugInList.Exists(p => p == plugIn))
                {
                    m_plugInList.Add(plugIn);
                }
            }
        }

        #endregion

        #region IPlugInEvents Members

        public event RequestStartedHandler RequestStarted;

        public event RequestStoppedHandler RequestStopped;

        #endregion
    }
}