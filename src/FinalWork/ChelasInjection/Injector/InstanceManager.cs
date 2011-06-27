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
        /// <summary>
        /// Indexer of a type
        /// </summary>
        public struct TypeIndex
        {
            public Type Type;
            public Type Attribute;
        }

        /// <summary>
        /// Auxiliary class for managing the creation of instances
        /// </summary>
        private class InstanceManager
        {
            private Binder m_binder;
            private List<TypeIndex> m_typeContructionPath = new List<TypeIndex>();
            private Dictionary<Type, Func<object>> m_notBindedConstructorList = new Dictionary<Type,Func<object>>();
            IActivationPlugIn m_defaultActivationObject = PerRequestInstanceManager.Singleton;

            public InstanceManager(Binder binder)
            {
                this.m_binder = binder;
            }

            internal object GetInstance(TypeIndex tIndex)
            {
                StartGetInstance(tIndex);
                object instance;
                Binder.BaseTypeConfig cTarget = GetBinderConfig(tIndex);
                if (cTarget != null && cTarget.ActivationObject != null)
                {
                     instance = cTarget.ActivationObject.GetInstance(tIndex);
                }
                else
                {
                    instance = m_defaultActivationObject.GetInstance(tIndex);
                }
                if (instance == null)
                {
                    instance = CreateInstance(tIndex, cTarget);
                    if (cTarget != null && cTarget.ActivationObject != null)
                    {
                        cTarget.ActivationObject.NewInstance(tIndex, instance);
                    }
                    else
                    {
                        m_defaultActivationObject.NewInstance(tIndex, instance);
                    }
                }
                StopGetInstance(tIndex);
                return instance;
            }

            private void StartGetInstance(TypeIndex tIndex)
            {
                if (m_typeContructionPath.Contains(tIndex))
                {
                    throw new Exceptions.CircularDependencyException();
                }
                else
                {
                    m_typeContructionPath.Add(tIndex);
                }
            }

            private void StopGetInstance(TypeIndex tIndex)
            {
                m_typeContructionPath.Remove(tIndex);
            }


            private object CreateInstance(TypeIndex tIndex, Binder.BaseTypeConfig cTarget)
            {
                // try to resolve the type from the custom resolvers
                object resolvedObject = m_binder.ResolveType(tIndex.Type);
                if (resolvedObject != null)
                {
                    CheckConfigForObject(cTarget, resolvedObject);
                    return resolvedObject;
                }

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
                    return m_notBindedConstructorList[tIndex.Type]();
                }
                else
                {
                    // create the object from the binded configuration
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
                if (chosenConstructor == null)
                {
                    throw new Exceptions.UnboundTypeException();
                }
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

            private void CheckConfigForObject(Binder.BaseTypeConfig tConfig, object resolvedObject)
            {
                if (tConfig == null)
                    return;
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
