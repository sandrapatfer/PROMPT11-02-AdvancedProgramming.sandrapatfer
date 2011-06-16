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
            private Binder m_Binder;
            private List<Type> m_typeContructionPath = new List<Type>();
            private Dictionary<Type, object> m_singletonList = new Dictionary<Type, object>();

            public InstanceManager(Binder binder)
            {
                this.m_Binder = binder;
            }

            internal T GetInstance<T>()
            {
                return (T)GetInstance(typeof(T));
            }

            internal object GetInstance(Type tType)
            {
                Binder.BaseTypeConfig cTarget = m_Binder.GetTargetType(tType);
                if (cTarget == null)
                {
                    ConstructorInfo constructor = FindConstructorWithMoreBindedObjects(tType);
                    if (constructor == null)
                    {
                        throw new Exceptions.UnboundTypeException();
                    }
                    else
                    {
                        AddType(tType);
                        object obj = CreateObjectWithConstructor(constructor);
                        RemoveType(tType);
                        return obj;
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
                        AddType(tType);
                        object obj = CreateObject(cTarget);
                        m_singletonList.Add(tType, obj);
                        RemoveType(tType);
                        return obj;

                    }
                }
                else
                {
                    AddType(tType);
                    object obj = CreateObject(cTarget);
                    RemoveType(tType);
                    return obj;
                }
            }

            private void AddType(Type tType)
            {
                if (m_typeContructionPath.Exists(t => t == tType))
                {
                    throw new Exceptions.CircularDependencyException();
                }
                else
                {
                    m_typeContructionPath.Add(tType);
                }
            }
            private void RemoveType(Type tType)
            {
                m_typeContructionPath.Remove(tType);
            }

            private object CreateObject(Binder.BaseTypeConfig cTarget)
            {
                switch (cTarget.ConstructorType)
                {
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoArguments:
                        {
                            var constructor = cTarget.TargetType.GetConstructor(Type.EmptyTypes);
                            // TODO check if exists
                            return constructor.Invoke(null);
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Values:
                        {
                            var constructor = cTarget.TargetType.GetConstructor(cTarget.ConstructorArguments);
                            // TODO check if exists
                            var args = new object[cTarget.ConstructorArguments.Length];
                            var values = cTarget.ConstructorValues();
                            var valuesObjectProperties = values.GetType().GetProperties();
                            foreach (var arg in cTarget.ConstructorArguments)
                            {
                            }
                            return constructor.Invoke(args);
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Action:
                        {
                            var obj = CreateObjectWithConstructor(FindConstructorWithMoreBindedObjects(cTarget.TargetType));
                            cTarget.ConstructorAction.Invoke(obj);
                            return obj;
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoValue:
                        {
                            return CreateObjectWithConstructor(FindConstructorWithMoreBindedObjects(cTarget.TargetType));
                        }
                    default:
                        throw new ArgumentException("Unexpected value in Binder.BaseTypeConfig.ConstructorTypeConfig");
                }
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
                        if (m_Binder.GetTargetType(param.ParameterType) != null)
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

            private object CreateObjectWithConstructor(ConstructorInfo constructor)
            {
                var args = new List<object>();
                foreach (var param in constructor.GetParameters())
                {
                    args.Add(GetInstance(param.ParameterType));
                }
                return constructor.Invoke(args.ToArray());
            }
        }
    }
}
