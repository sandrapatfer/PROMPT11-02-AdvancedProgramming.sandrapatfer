using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    public interface IActivationPlugIn
    {
        void Init(IPlugInEvents eventController);
        object GetInstance(Injector.TypeIndex typeIndex);
        void NewInstance(Injector.TypeIndex typeIndex, object instance);
    }
}
