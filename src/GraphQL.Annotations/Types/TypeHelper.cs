using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Annotations.Types
{
    public static class TypeHelper
    {
        public static void ApplyMetadata<TModelType>(this GraphType instance)
        {
            var type = typeof (TModelType);
            var metadata = type.GetTypeInfo().GetCustomAttribute<GraphQLTypeAttribute>();

            if (metadata == null)
            {
                var message = string.Format("{0} is not marked as a GraphQL type - did you forget to mark it with a GraphQLTypeAttribute?", type.Name);
                throw new NotSupportedException(message);
            }

            instance.Name = !string.IsNullOrWhiteSpace(metadata.Name) ? metadata.Name : type.Name;
            instance.Description = metadata.Description;
        }

        public static void ImplementFields<TModelType>(this ComplexGraphType<TModelType> instance)
        {
            var type = typeof (TModelType);
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance))
            {
                var fieldAttr = prop.GetCustomAttribute<GraphQLFieldAttribute>();
                if (fieldAttr == null)
                    continue;

                instance.Field(
                    fieldAttr.ReturnType ?? prop.PropertyType.ToGraphType(),
                    fieldAttr.Name ?? prop.Name.FirstCharacterToLower(),
                    fieldAttr.Description,
                    resolve: context => prop.GetValue(context.Source, null)
                );
            }
        }
        
        public static void ImplementFuncs<TModelType>(this ComplexGraphType<TModelType> instance, Func<MethodInfo, Dictionary<ParameterInfo, QueryArgument>, IFieldResolver> resolverFactory = null)
        {
            var type = typeof (TModelType);

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(m => !m.IsSpecialName))
            {
                var funcAttr = method.GetCustomAttribute<GraphQLFuncAttribute>();
                if (funcAttr == null)
                    continue;

                var methodDescription = string.Format("`{0}` on type '{1}'", method.Name, type.Name);
                var methodParams = method.GetParameters();
                var parameterArgumentMappings = new Dictionary<ParameterInfo, QueryArgument>();

                // Ensure arguments have the attributes
                var queryParameters = methodParams.Skip(1).ToArray();
                foreach (var param in queryParameters)
                {
                    var paramAttr = param.GetCustomAttribute<GraphQLArgumentAttribute>();
                    if (paramAttr == null)
                        throw new NotSupportedException(
                            string.Format(
                                "Parameter `{0}` in method {1} is missing a required GraphQLFuncParamAttribute",
                                param.Name, methodDescription));

                    parameterArgumentMappings.Add(param, new QueryArgument(param.ParameterType)
                    {
                        Name = paramAttr.Name ?? param.Name,
                        Description = paramAttr.Description
                    });
                }

                // Void methods aren't allowed
                if (method.ReturnType == typeof(void))
                    throw new NotSupportedException(
                        string.Format(
                            "Invalid return type `void` for {0} - GraphQL methods must return values as they are used as getters.",
                            methodDescription));

                // Create the field
                instance.AddField(new FieldType
                {
                    Type = funcAttr.ReturnType ?? method.ReturnType.ToGraphType(),
                    Name = funcAttr.Name ?? method.Name.FirstCharacterToLower(),
                    Description = funcAttr.Description,
                    Arguments = new QueryArguments(parameterArgumentMappings.Values),
                    Resolver = resolverFactory?.Invoke(method, parameterArgumentMappings)
                });
            }
        }

        private static string FirstCharacterToLower(this string s)
        {
            return s.Substring(0,1).ToLower() + s.Skip(1);
        }
    }
}
