using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace ChelasInjection
{
    partial class Injector
    {
        class InstanceManager
        {
            private Binder m_binder;
            private List<Type> m_typeContructionPath = new List<Type>();
            private Dictionary<Type, object> m_singletonList = new Dictionary<Type, object>();
            private Dictionary<Type, object> m_currentCallObjectList;

            public InstanceManager(Binder binder)
            {
                this.m_binder = binder;
            }

            internal T GetInstance<T>()
            {
                m_currentCallObjectList = new Dictionary<Type, object>();
                return (T)GetInstance(typeof(T));
            }

            internal object GetInstance(Type tType)
            {
                // a type already created is always returned
                if (m_currentCallObjectList.ContainsKey(tType))
                    return m_currentCallObjectList[tType];

                // a type configured as singleton always returns the same type, independently of the configuration
                Binder.BaseTypeConfig cTarget = m_binder.GetTargetType(tType);
                if (cTarget != null && cTarget.ActivationSingleton)
                {
                    if (m_singletonList.ContainsKey(tType))
                    {
                        return m_singletonList[tType];
                    }
                }

                object resolvedObject = m_binder.ResolveType(tType);
                if (resolvedObject != null)
                {
                    CheckConfigForObject(tType, resolvedObject);
                }
                else
                {
                    resolvedObject = GetInstanceFromConfig(tType);
                }
                m_currentCallObjectList.Add(tType, resolvedObject);
                return resolvedObject;
            }

            internal object GetInstanceFromConfig(Type tType)
            {
                Binder.BaseTypeConfig cTarget = m_binder.GetTargetType(tType);
                if (cTarget == null)
                {
                    ConstructorInfo constructor = FindConstructor(tType);
                    if (constructor == null)
                    {
                        throw new Exceptions.UnboundTypeException();
                    }
                    else
                    {
                        return ProtectObjectCreation(tType, ()=>CreateObjectWithConstructor(constructor, c => null), null);
                    }
                }
                else if (cTarget.ActivationSingleton)
                {
                    if (m_singletonList.ContainsKey(tType))
                    {
                        return m_singletonList[tType];
                    }
                    else
                    {
                        return ProtectObjectCreation(tType, ()=> CreateObject(cTarget), obj => m_singletonList.Add(tType, obj));
                    }
                }
                else
                {
                    return ProtectObjectCreation(tType, ()=>CreateObject(cTarget), null);
                }
            }

            private object ProtectObjectCreation(Type tType, Func<object> creationFunc, Action<object> additionalAction)
            {
                if (m_typeContructionPath.Exists(t => t == tType))
                {
                    throw new Exceptions.CircularDependencyException();
                }
                else
                {
                    m_typeContructionPath.Add(tType);
                }

                object obj = creationFunc();
                if (additionalAction != null)
                    additionalAction(obj);

                m_typeContructionPath.Remove(tType);
                
                return obj;
            }

            private object CreateObject(Binder.BaseTypeConfig cTarget)
            {
                switch (cTarget.ConstructorType)
                {
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoArguments:
                        {
                            return CreateObjectWithConstructor(cTarget.TargetType.GetConstructor(Type.EmptyTypes), constructor => null);
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Values:
                        {
                            return CreateObjectWithConstructor(cTarget.TargetType.GetConstructor(cTarget.ConstructorArguments),
                                constructor => CreateArguments(constructor, cTarget.ConstructorValues()));
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Action:
                        {
                            var obj = CreateObjectWithConstructor(FindConstructor(cTarget.TargetType),
                                constructor => constructor.GetParameters().Select(p => GetInstance(p.ParameterType)).ToArray());
                            cTarget.ConstructorAction(obj);
                            return obj;
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoValue:
                        {
                            return CreateObjectWithConstructor(FindConstructor(cTarget.TargetType),
                                constructor => constructor.GetParameters().Select(p => GetInstance(p.ParameterType)).ToArray());
                        }
                    default:
                        throw new ArgumentException("Unexpected value in Binder.BaseTypeConfig.ConstructorTypeConfig");
                }
            }

            private ConstructorInfo FindConstructor(Type objectType)
            {
                var constructor = FindConstructorMarkedWithDefaultAttr(objectType);
                if (constructor == null)
                {
                    constructor = FindConstructorWithMoreBindedObjects(objectType);
                }
                return constructor;
            }

            private ConstructorInfo FindConstructorMarkedWithDefaultAttr(Type objectType)
            {
                return objectType.GetConstructors().FirstWhere(c => c.GetCustomAttributes(typeof(DefaultConstructorAttribute), false).Length == 1);
            }

            private ConstructorInfo FindConstructorWithMoreBindedObjects(Type objectType)
            {
                ConstructorInfo chosenConstructor = null;
                int chosenConstructorNBindedAttrs = -1;
                foreach (var constructor in objectType.GetConstructors())
                {
                    int NBindedAttrs = 0;
                    foreach (var param in constructor.GetParameters())
                    {
                        if (m_binder.GetTargetType(param.ParameterType) != null)
                        {
                            NBindedAttrs++;
                        }
                        else
                        {
                            NBindedAttrs = -1;
                            break;
                        }
                    }
                    if (NBindedAttrs != -1 && (chosenConstructorNBindedAttrs == -1 || NBindedAttrs > chosenConstructorNBindedAttrs))
                    {
                        chosenConstructorNBindedAttrs = NBindedAttrs;
                        chosenConstructor = constructor;
                    }
                }
                //TODO excepcao se nao existir
                return chosenConstructor;
            }

            private object CreateObjectWithConstructor(ConstructorInfo constructor, Func<ConstructorInfo, object[]> parameters)
            {
                if (constructor == null)
                    throw new Exceptions.MissingAppropriateConstructorException();
                return constructor.Invoke(parameters(constructor));
            }
            
            private object[] CreateArguments(ConstructorInfo constructor, object constructorValuesObject)
            {
                return constructor.GetParameters().Select(p => CreateParameterObject(p, constructorValuesObject)).ToArray();
            }

            private object CreateParameterObject(ParameterInfo param, object constructorValuesObject)
            {
                var valuesObjectProperties = constructorValuesObject.GetType().GetProperties();
                var paramProperty = valuesObjectProperties.FirstWhere(p => p.PropertyType == param.ParameterType);
                if (paramProperty != null)
                {
                    return paramProperty.GetValue(constructorValuesObject, null);
                }
                else
                {
                    return GetInstance(param.ParameterType);
                }
            }

            private void CheckConfigForObject(Type tType, object resolvedObject)
            {
                Binder.BaseTypeConfig cTarget = m_binder.GetTargetType(tType);
                if (cTarget == null)
                    return;
                if (cTarget.ActivationSingleton)
                {
                    m_singletonList.Add(tType, resolvedObject);
                }

                if (cTarget.ConstructorType == Binder.BaseTypeConfig.ConstructorTypeConfig.Action)
                {
                    cTarget.ConstructorAction(resolvedObject);
                }
            }
        }
    }
}
