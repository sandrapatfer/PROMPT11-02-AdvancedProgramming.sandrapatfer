using System;
using System.Collections.Generic;

namespace ChelasInjection
{
    /// <summary>
    /// The Injector is responsible for handling the GetInstance requests
    /// Also provides events for the starting and ending of each request to the plug ins
    /// </summary>
    public partial class Injector : IPlugInEvents
    {
        private Binder _myBinder;
        private InstanceManager _myInstanceManager;

        public Injector(Binder myBinder)
        {
            _myBinder = myBinder;
            _myBinder.NewPlugIn += new NewPlugInHandler(_myBinder_NewPlugIn);
            _myBinder.Configure();
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

        private List<IActivationPlugIn> m_plugInList = new List<IActivationPlugIn>();

        void _myBinder_NewPlugIn(Binder sender, IActivationPlugIn plugIn)
        {
            if (plugIn != null)
            {
                if (!m_plugInList.Exists(p => p == plugIn))
                {
                    m_plugInList.Add(plugIn);
                }
            }
        }

        #region IPlugInEvents Members

        public event RequestStartedHandler RequestStarted;

        public event RequestStoppedHandler RequestStopped;

        #endregion
    }
}