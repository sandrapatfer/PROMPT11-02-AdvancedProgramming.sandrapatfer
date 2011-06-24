using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    public interface IPlugInController
    {
        void NewPlugIn(IActivationPlugIn plugIn);
    }
}
