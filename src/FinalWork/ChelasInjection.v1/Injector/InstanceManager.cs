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
            private Dictionary<Type, Func<object>> m_notBindedConstructorList = new Dictionary<Type,Func<object>>();

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
                    if (cTarget == null)
                    {
                        // for not binded types, find the most suitable constructor
                        if (!m_notBindedConstructorList.ContainsKey(tIndex.Type))
                        {
                            ConstructorInfo constructor = FindConstructor(tIndex.Type);
                            if (constructor == null)
                            {
                                throw new Exceptions.UnboundTypeException();
                            }
                            else
                            {
                                var expr = CreateExpressionConstructor(constructor,
                                    ci => ci.GetParameters().Select(p => CreateParameterExpression(p)).ToArray());
                                m_notBindedConstructorList.Add(tIndex.Type, Expression.Lambda<Func<object>>(expr).Compile());
                            }
                        }
                        return ProtectObjectCreation(tIndex, () => m_notBindedConstructorList[tIndex.Type](), null);
                    }
                    else
                    {
                        // create the object from the binded configuration
                        resolvedObject = GetInstanceFromConfig(tIndex, cTarget);
                    }
                }

                m_currentCallObjectList.Add(tIndex, resolvedObject);
                return resolvedObject;
            }

            private object GetInstanceFromConfig(TypeIndex tIndex, Binder.BaseTypeConfig tConfig)
            {
                if (tConfig.ActivationSingleton)
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
                if (cTarget.Constructor == null)
                {
                    CreateConstructor(cTarget);
                }

                var obj = cTarget.Constructor();
                if (cTarget.ConstructorType == Binder.BaseTypeConfig.ConstructorTypeConfig.Action)
                {
                    cTarget.ConstructorAction(obj);
                }
                return obj;
            }

            private void CreateConstructor(Binder.BaseTypeConfig cTarget)
            {
                Expression expr;
                switch (cTarget.ConstructorType)
                {
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoArguments:
                        {
                            expr = Expression.New(cTarget.TargetType);
                            break;
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Values:
                        {
                            expr = CreateExpressionConstructor(cTarget.TargetType.GetConstructor(cTarget.ConstructorArguments),
                                ci => CreateExpressions(ci, cTarget.ConstructorValues()));
                            break;
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.Action:
                        {
                            expr = CreateExpressionConstructor(FindConstructor(cTarget.TargetType),
                                ci => ci.GetParameters().Select(p => CreateParameterExpression(p)).ToArray());
                            break;
                        }
                    case Binder.BaseTypeConfig.ConstructorTypeConfig.NoValue:
                        {
                            expr = CreateExpressionConstructor(FindConstructor(cTarget.TargetType),
                                ci => ci.GetParameters().Select(p => CreateParameterExpression(p)).ToArray());
                            break;
                        }
                    default:
                        throw new ArgumentException("Unexpected value in Binder.BaseTypeConfig.ConstructorTypeConfig");
                }
                cTarget.Constructor = Expression.Lambda<Func<object>>(expr).Compile();
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

            private Expression CreateExpressionConstructor(ConstructorInfo constructor, Func<ConstructorInfo, Expression[]> expressions)
            {
                if (constructor == null)
                    throw new Exceptions.MissingAppropriateConstructorException();
                return Expression.New(constructor, expressions(constructor));
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

            private Expression CreateParameterExpression(ParameterInfo param)
            {
                return Expression.TypeAs(Expression.Call(Expression.Constant(this), typeof(InstanceManager).GetMethod("GetInstance", BindingFlags.NonPublic | BindingFlags.Instance, Type.DefaultBinder, new Type[] { typeof(TypeIndex) }, null),
                    GetParameterTypeIndexExpression(param)), param.ParameterType);
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
