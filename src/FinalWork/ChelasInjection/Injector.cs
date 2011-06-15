using System;

namespace ChelasInjection
{
    public partial class Injector
    {
        private Binder _myBinder;
        private InstanceManager _myInstanceManager;

        public Injector(Binder myBinder)
        {
            _myBinder = myBinder;
            _myBinder.Configure();
            _myInstanceManager = new InstanceManager(_myBinder);
        }

        public T GetInstance<T>()
        {
            return _myInstanceManager.GetInstance<T>();
        }
    }
}