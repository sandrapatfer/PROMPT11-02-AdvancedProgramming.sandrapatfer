using System;
using System.Collections.Generic;

namespace ChelasInjection
{
    public delegate object ResolverHandler(Binder sender, Type t);

    public abstract partial class Binder
    {
        public void Configure()
        {
            InternalConfigure();
        }

        protected abstract void InternalConfigure();

        public event ResolverHandler CustomResolver;

        Dictionary<Type, BaseTypeConfig> m_bindingTypes = new Dictionary<Type,BaseTypeConfig>();

        public ITypeBinder<Target> Bind<Source, Target>()
        {
            Type sType = typeof(Source);
            var tConfig = new TypeConfig<Target>(typeof(Target));
            if (m_bindingTypes.ContainsKey(sType))
            {
                // replaces the current configuration ?
                m_bindingTypes[sType] = tConfig;
            }
            else
            {
                m_bindingTypes.Add(sType, tConfig);
            }
            return tConfig;
        }

        public ITypeBinder<Source> Bind<Source>()
        {
            Type sType = typeof(Source);
            var tConfig = new TypeConfig<Source>(sType);
            if (m_bindingTypes.ContainsKey(sType))
            {
                // replaces the current configuration ?
                m_bindingTypes[sType] = tConfig;
            }
            else
            {
                m_bindingTypes.Add(sType, tConfig);
            }
            return tConfig;
        }

        internal BaseTypeConfig GetTargetType(Type sType)
        {
            if (m_bindingTypes.ContainsKey(sType))
            {
                return m_bindingTypes[sType];
            }
            else
            {
                return null;
            }
        }

    }
}