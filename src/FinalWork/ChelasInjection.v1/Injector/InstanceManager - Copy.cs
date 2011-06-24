using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace ChelasInjection
{
    partial class Injector
    {
        public struct TypeIndex
        {
            public Type Type;
            public Type Attribute;
        }

        class InstanceManager
        {
            private Binder m_binder;
            private List<TypeIndex> m_typeContructionPath = new List<TypeIndex>();
            private Dictionary<TypeIndex, object> m_singletonList = new Dictionary<TypeIndex, object>();
            private Dictionary<TypeIndex, object> m_currentCallObjectList;

            public InstanceManager(Binder binder)
            {
                this.m_binder = binder;
            }

            public T GetInstance<T>()
            {
                m_currentCallObjectList = new Dictionary<TypeIndex, object>();
                return (T)GetInstance(new TypeIndex() { Type = typeof(T) });
            }

            public T GetInstance<T, TA>()
            {
                m_currentCallObjectList = new Dictionary<TypeIndex, object>();
                return (T)GetInstance(new TypeIndex() { Type = typeof(T), Attribute = typeof(TA) });
            }

            internal object GetInstance(TypeIndex tIndex)
            {
                // a type already created is always returned
                if (m_currentCallObjectList.ContainsKey(tIndex))
                    return m_currentCallObjectList[tIndex];

                // a type configured as singleton always returns the same object, independently of the configuration
                Binder.BaseTypeConfig cTarget = GetBinderConfig(tIndex);
                if (cTarget != null && cTarget.ActivationSingleton)
                {
                    if (m_singletonList.ContainsKey(tIndex))
                    {
                        return m_singletonList[tIndex];
                    }
                }

                // try to resolve the type from the custom resolvers
                object resolvedObject = m_binder.ResolveType(tIndex.Type);
                if (resolvedObject != null)
                {
                    CheckConfigForObject(tIndex, cTarget, resolvedObject);
                }
                else
                {
                    // create the object from the binded configuration
                    resolvedObject = GetInstanceFromConfig(tIndex, cTarget);
                }

                m_currentCallObjectList.Add(tIndex, resolvedObject);
                return resolvedObject;
            }

            private object GetInstanceFromConfig(TypeIndex tIndex, Binder.BaseTypeConfig tConfig)
            {
                if (tConfig == null)
                {
                    ConstructorInfo constructor = FindConstructor(tIndex.Type);
                    if (constructor == null)
                    {
                        throw new Exceptions.UnboundTypeException();
                    }
                    else
                    {
                        return ProtectObjectCreation(tIndex, () => CreateObjectWithConstructor(constructor, FuncForBuildingParameters()), null);
                    }
                }
                else if (tConfig.ActivationSingleton)
                {
                    return ProtectObjectCreation(tIndex, () => CreateObject(tConfig), obj => m_singletonList.Add(tIndex, obj));
                }
                else
                {
                    return ProtectObjectCreation(tIndex, () => CreateObject(tConfig), null);
                }
            }

            private object ProtectObjectCreation(TypeIndex tIndex, Func<object> creationFunc, Action<object> additionalAction)
            {
                if (m_typeContructionPath.Exists(t => t.Type == tIndex.Type && t.Attribute == tIndex.Attribute))
                {
                    throw new Exceptions.CircularDependencyException();
                }
                else
                {
                    m_typeContructionPath.Add(tIndex);
                }

                object obj = creationFunc();
                if (additionalAction != null)
                    additionalAction(obj);

                m_typeContructionPath.Remove(tIndex);
                
                return obj;
            }

            private object CreateObject(Binder.BaseTypeConfig cTarget)
            {
                if (cTarget.Constructor != null)
                {
                    return cTarget.Constructor();
                }

                switch (cTarget.ConstructorType)
                {
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoArguments:
                        {
                            var expr = Expression.New(cTarget.TargetType);
                            cTarget.Constructor = Expression.Lambda<Func<object>>(expr).Compile();
                            return cTarget.Constructor();
//                            return CreateObjectWithConstructor(cTarget.TargetType.GetConstructor(Type.EmptyTypes), constructor => null);
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Values:
                        {
                            ConstructorInfo ci = cTarget.TargetType.GetConstructor(cTarget.ConstructorArguments);
                            var exprs = CreateExpressions(ci, cTarget.ConstructorValues());
                            var expr = Expression.New(ci, exprs);
                            cTarget.Constructor = Expression.Lambda<Func<object>>(expr).Compile();
                            return cTarget.Constructor();
//                            return CreateObjectWithConstructor(cTarget.TargetType.GetConstructor(cTarget.ConstructorArguments),
//                                constructor => CreateArguments(constructor, cTarget.ConstructorValues()));
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Action:
                        {
                            var obj = CreateObjectWithConstructor(FindConstructor(cTarget.TargetType), FuncForBuildingParameters());
                            cTarget.ConstructorAction(obj);
                            return obj;
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoValue:
                        {
                            return CreateObjectWithConstructor(FindConstructor(cTarget.TargetType), FuncForBuildingParameters());
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
                var constructors = objectType.GetConstructors().Where(c => c.GetCustomAttributes(typeof(DefaultConstructorAttribute), false).Length == 1);
                if (constructors.Count() == 0)
                    return null;
                else if (constructors.Count() == 1)
                    return constructors.ElementAt(0);
                else
                    throw new Exceptions.MultipleDefaultConstructorAttributesException();
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

            private Func<ConstructorInfo, object[]> FuncForBuildingParameters()
            {
                return constructor => constructor.GetParameters().Select(p => GetInstance(GetParameterTypeIndex(p))).ToArray();
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
                    return GetInstance(GetParameterTypeIndex(param));
                }
            }

            private Expression[] CreateExpressions(ConstructorInfo constructor, object constructorValuesObject)
            {
                return constructor.GetParameters().Select(p => CreateParameterExpression(p, constructorValuesObject)).ToArray();
            }

            private Expression CreateParameterExpression(ParameterInfo param, object constructorValuesObject)
            {
                var valuesObjectProperties = constructorValuesObject.GetType().GetProperties();
                var paramProperty = valuesObjectProperties.FirstWhere(p => p.PropertyType == param.ParameterType);
                if (paramProperty != null)
                {
                    return Expression.Property(Expression.Constant(constructorValuesObject), paramProperty);
                }
                else
                {
                    return Expression.TypeAs(Expression.Call(Expression.Constant(this), typeof(InstanceManager).GetMethod("GetInstance", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(TypeIndex) }, null),
                        GetParameterTypeIndexExpression(param)), param.ParameterType);
                }
            }

            private TypeIndex GetParameterTypeIndex(ParameterInfo param)
            {
                object[] attrs = param.GetCustomAttributes(false);
                if (attrs.Length == 1)
                {
                    return new TypeIndex() { Type = param.ParameterType, Attribute = attrs[0].GetType() };
                }
                else
                {
                    return new TypeIndex() { Type = param.ParameterType };
                }
            }

            private Expression GetParameterTypeIndexExpression(ParameterInfo param)
            {
                var exprNew = Expression.New(typeof(TypeIndex));
                MemberBinding[] bindings;
                object[] attrs = param.GetCustomAttributes(false);
                if (attrs.Length == 1)
                {
                    bindings = new MemberBinding[] { Expression.Bind(typeof(TypeIndex).GetField("Type"), Expression.Constant(param.ParameterType)),
                                       Expression.Bind(typeof(TypeIndex).GetField("Attribute"), Expression.Constant(attrs[0].GetType())) };
                }
                else
                {
                    bindings = new MemberBinding[] { Expression.Bind(typeof(TypeIndex).GetField("Type"), Expression.Constant(param.ParameterType)) };
                }
                return Expression.MemberInit(exprNew, bindings);
            }

            private void CheckConfigForObject(TypeIndex tIndex, Binder.BaseTypeConfig tConfig, object resolvedObject)
            {
                if (tConfig == null)
                    return;
                if (tConfig.ActivationSingleton)
                {
                    m_singletonList.Add(tIndex, resolvedObject);
                }
                if (tConfig.ConstructorType == Binder.BaseTypeConfig.ConstructorTypeConfig.Action)
                {
                    tConfig.ConstructorAction(resolvedObject);
                }
            }

            private Binder.BaseTypeConfig GetBinderConfig(TypeIndex tIndex)
            {
                Binder.TypeConfigHandler tHandler = m_binder.GetTargetType(tIndex.Type);
                if (tHandler != null)
                {
                    if (tIndex.Attribute == null)
                    {
                        return tHandler.DefaultConfig;
                    }
                    else
                    {
                        Binder.BaseTypeConfig tConfig = tHandler.AttributeConfig(tIndex.Attribute);
                        if (tConfig != null)
                        {
                            return tConfig;
                        }
                        else
                        {
                            // use the default config when the constructor has a non existing attribute
                            return tHandler.DefaultConfig;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
