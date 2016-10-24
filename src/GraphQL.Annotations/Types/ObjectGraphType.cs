using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Annotations.Types
{
    public class ObjectGraphType<TModelType> : GraphQL.Types.ObjectGraphType<TModelType>
        where TModelType : class
    {
        public ObjectGraphType()
        {
            this.ApplyMetadata<TModelType>();
            this.ImplementFields();
            this.ImplementFuncs(CreateResolveForMethod);
            ImplementInterfaces();
        }

        private static IFieldResolver CreateResolveForMethod(MethodInfo method, Dictionary<ParameterInfo, QueryArgument> argumentMap)
        {
            var methodParams = method.GetParameters();
            var parameterExpressions = methodParams.Select(p => Expression.Parameter(p.ParameterType, p.Name));
            var methodExpression = method.IsStatic
                ? Expression.Call(method, parameterExpressions)
                : Expression.Call(Expression.Parameter(typeof (TModelType)), method, parameterExpressions);

            Func<ResolveFieldContext<TModelType>,object> resolve = context =>
            {
                var arguments = new object[methodParams.Length];

                // Context is always first argument
                arguments[0] = context;

                // then any query arguments.
                foreach (var param in argumentMap)
                {
                    arguments[param.Key.Position] =
                        ConvertArgument(
                            context.GetArgument<object>(param.Value.Name),
                            param.Key.ParameterType);
                }

                return methodExpression.Method.Invoke(method.IsStatic ? null : context.Source, arguments);
            };

            return new FuncFieldResolver<TModelType,object>(resolve);
        } 

        private void ImplementInterfaces()
        {
            var type = typeof (TModelType);
            var baseTypes = GetBaseTypes(type).Where(t => t.GetTypeInfo().IsAbstract);
            var interfaces = type.GetInterfaces();
            var implementedInterfaces = baseTypes.Concat(interfaces);
            var genericInterfaceType = typeof(InterfaceGraphType<>);
            foreach (var implementedInterface in implementedInterfaces)
            {
                var interfaceAttr = implementedInterface.GetTypeInfo().GetCustomAttribute<GraphQLInterfaceAttribute>(false);
                if (interfaceAttr == null)
                    continue;

                // Call Interface(typeof(InterfaceGraphType<Interface>));
                Interface(genericInterfaceType.MakeGenericType(implementedInterface));
            }
        }

        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            while (type.GetTypeInfo().BaseType != null)
            {
                type = type.GetTypeInfo().BaseType;
                yield return type;
            }
        }

        public override string ToString()
        {
            return Name + " - Object Type";
        }

        private static object ConvertArgument(object argument, Type type)
        {
            // object[] => T[]
            var rawArray = argument as object[];
            if (rawArray != null)
            {
                var arrayType = type.GetElementType();
                var typedArray = Array.CreateInstance(arrayType, rawArray.Length);
                Array.Copy(rawArray, typedArray, rawArray.Length);
                return typedArray;
            }

            // Dictionary<string,object> => T
            var dictionary = argument as Dictionary<string,object>;
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
