using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChelasInjection
{
    public partial class Binder
    {
        internal class TypeConfigHandler
        {
            protected BaseTypeConfig m_defaultConfig;
            private Dictionary<Type, BaseTypeConfig> m_configByAttributes;

            public TypeConfigHandler()
            {}
            
            public void HandleNewConfig(Binder.BaseTypeConfig config)
            {
                if (config.AttributeType == null)
                {
                    // always use the last config for the same type, so we just ignore the previous, if exists
                    m_defaultConfig = config;
                }
                else
                {
                    if (m_configByAttributes == null)
                    {
                        m_configByAttributes = new Dictionary<Type, BaseTypeConfig>();
                    }
                    if (m_configByAttributes.ContainsKey(config.AttributeType))
                    {
                        throw new Exceptions.MultipleDefaultConstructorAttributesException();
                    }
                    else
                    {
                        m_configByAttributes.Add(config.AttributeType, config);
                    }
                }
            }

            public Binder.BaseTypeConfig DefaultConfig { get { return m_defaultConfig; } }

            public Binder.BaseTypeConfig AttributeConfig(Type attrType)
            {
                if (m_configByAttributes.ContainsKey(attrType))
                {
                    return m_configByAttributes[attrType];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
