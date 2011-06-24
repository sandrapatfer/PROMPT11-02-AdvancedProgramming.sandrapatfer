using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    public delegate void RequestStartedHandler(IPlugInEvents sender, Injector.TypeIndex t);
    public delegate void RequestStoppedHandler(IPlugInEvents sender, Injector.TypeIndex t);

    public interface IPlugInEvents
    {
        event RequestStartedHandler RequestStarted;

        event RequestStoppedHandler RequestStopped;
    }
}
