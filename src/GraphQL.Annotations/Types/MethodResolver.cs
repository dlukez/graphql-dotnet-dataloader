using System;
using System.Collections.Generic;
using System.Reflection;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Annotations.Resolvers
{
    public class MethodResolver<TSource> : IFieldResolver<TSource>
    {
        private readonly MethodInfo m_MethodInfo;
        //private readonly MethodCallExpression m_MethodExpression;
        private readonly Dictionary<ParameterInfo, QueryArgument> m_Arguments;

        public MethodResolver(MethodInfo methodInfo, Dictionary<ParameterInfo, QueryArgument> argumentMap)
        {
            m_MethodInfo = methodInfo;
            //var methodParams = methodInfo.GetParameters();
            //var parameterExpressions = methodParams.Select(p => Expression.Parameter(p.ParameterType, p.Name));
            //m_MethodExpression = methodInfo.IsStatic
            //    ? Expression.Call(methodInfo, parameterExpressions)
            //    : Expression.Call(Expression.Parameter(typeof(TSource)), methodInfo, parameterExpressions);
            m_Arguments = argumentMap;
        }

        private TSource ResolveInternal(ResolveFieldContext context)
        {
            var arguments = new object[m_Arguments.Count + 1];

            // Context is always first argument
            arguments[0] = new ResolveFieldContext<TSource>(context);

            // then any query arguments.
            foreach (var param in m_Arguments)
            {
                arguments[param.Key.Position] =
                    ConvertArgument(
                        context.GetArgument<object>(param.Value.Name),
                        param.Key.ParameterType);
            }

            return (TSource)m_MethodInfo.Invoke(null, arguments);
        }

        public TSource Resolve(ResolveFieldContext context)
        {
            return ResolveInternal(context);
        }

        object IFieldResolver.Resolve(ResolveFieldContext context)
        {
            return ResolveInternal(context);
        }

        private static object ConvertArgument(object argument, Type type)
        {
            // object[] => T[]
            //var rawArray = argument as object[];
            //if (rawArray != null)
            //{
            //    var arrayType = type.GetElementType();
            //    var typedArray = Array.CreateInstance(arrayType, rawArray.Length);
            //    Array.Copy(rawArray, typedArray, rawArray.Length);
            //    return typedArray;
            //}

            // Dictionary<string, object> => T
            var dictionary = argument as Dictionary<string, object>;
            if (dictionary != null)
            {
                var instance = Activator.CreateInstance(type);
                foreach (var kvp in dictionary)
                {
                    var property = type.GetProperty(kvp.Key,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    property.SetValue(instance, ConvertArgument(kvp.Value, property.PropertyType));
                }
                return instance;
            }

            return argument;
        }
    }
}
