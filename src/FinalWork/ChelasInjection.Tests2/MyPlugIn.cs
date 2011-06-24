using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    class MyPlugIn : IActivationPlugIn
    {
        INew1 m_currentObject;

        #region IActivationPlugIn Members

        public void Init(IPlugInEvents eventController)
        {
            eventController.RequestStarted += new RequestStartedHandler(eventController_RequestStarted);
            eventController.RequestStopped += new RequestStoppedHandler(eventController_RequestStopped);
        }

        public object GetInstance(Injector.TypeIndex typeIndex)
        {
            return m_currentObject;
        }

        public void NewInstance(Injector.TypeIndex typeIndex, object instance)
        {
        }

        #endregion

        void eventController_RequestStarted(IPlugInEvents sender, Injector.TypeIndex t)
        {
            m_currentObject = new INew1(3);
        }

        void eventController_RequestStopped(IPlugInEvents sender, Injector.TypeIndex t)
        {
            m_currentObject = null;
        }
    }

    public static class PlugInUtils
    {
        public static ITypeBinder<T> MyPlugIn<T>(this IActivationBinder<T> typeBinder)
        {
            typeBinder.ActivationPlugIn = new MyPlugIn();
            return typeBinder.GetCurrentTypeBinder;
        }
    }

    public class INew1
    {
        public int TheInt { get; internal set; }
        public INew1(int i)
        {
            TheInt = i;
        }
    }
}
