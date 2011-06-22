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
            m_activeConfigSourceType = null;
            m_activeConfig = null;
            InternalConfigure();
            HandleActiveConfig();
        }

        protected abstract void InternalConfigure();

        Dictionary<Type, TypeConfigHandler> m_bindingTypes = new Dictionary<Type, TypeConfigHandler>();
        Type m_activeConfigSourceType;
        BaseTypeConfig m_activeConfig;

        public ITypeBinder<Target> Bind<Source, Target>()
        {
            HandleActiveConfig();

            m_activeConfigSourceType = typeof(Source);
            m_activeConfig = new TypeConfig<Target>(typeof(Target));
            return (TypeConfig<Target>)m_activeConfig;
        }

        public ITypeBinder<Source> Bind<Source>()
        {
            HandleActiveConfig();

            m_activeConfigSourceType = typeof(Source);
            m_activeConfig = new TypeConfig<Source>(typeof(Source));
            return (TypeConfig<Source>)m_activeConfig;
        }

        private void HandleActiveConfig()
        {
            if (m_activeConfigSourceType != null)
            {
                if (m_bindingTypes.ContainsKey(m_activeConfigSourceType))
                {
                    m_bindingTypes[m_activeConfigSourceType].HandleNewConfig(m_activeConfig);
                }
                else
                {
                    m_bindingTypes.Add(m_activeConfigSourceType, new TypeConfigHandler(m_activeConfig));
                }
            }
        }

        internal TypeConfigHandler GetTargetType(Type tType)
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