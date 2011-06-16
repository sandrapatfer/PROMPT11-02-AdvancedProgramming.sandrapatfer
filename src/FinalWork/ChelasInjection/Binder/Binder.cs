using System;
using System.Collections.Generic;
using System.Linq;

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

        internal BaseTypeConfig GetTargetType(Type tType)
        {
            if (m_bindingTypes.ContainsKey(tType))
            {
                return m_bindingTypes[tType];
            }
            else
            {
                return null;
            }
        }

        public event ResolverHandler CustomResolver;

        public object ResolveType(Type tType)
        {
            if (CustomResolver != null)
            {
                return CustomResolver.GetInvocationList().ReturnFirstNotNull(d => ((ResolverHandler)d)(this, tType));
            }
            else
            {
                return null;
            }
        }
    }
}