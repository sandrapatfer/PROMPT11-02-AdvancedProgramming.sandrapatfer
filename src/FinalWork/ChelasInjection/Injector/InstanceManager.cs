using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    partial class Injector
    {
        class InstanceManager
        {
            private Binder _myBinder;

            public InstanceManager(Binder binder)
            {
                this._myBinder = binder;
            }

            internal T GetInstance<T>()
            {
                Binder.BaseTypeConfig cTarget = _myBinder.GetTargetType(typeof(T));
                if (cTarget == null)
                {
                    throw new Exceptions.UnboundTypeException();
                }
                else
                {
                    if (cTarget.ActivationSingleton)
                        return (T)Activator.CreateInstance(cTarget.TargetType);

                    else
                    {

                    }
                }
            }
        }
    }
}
