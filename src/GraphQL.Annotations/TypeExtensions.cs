using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL;
using GraphQL.Types;

namespace Serraview.GraphQL.Annotations
{
    public static class TypeExtensions
    {
        public static Type ToGraphType(this Type type)
        {
            if (type.IsGraphType())
                return type;

            if (type.IsArray)
                return typeof(ListGraphType<>).MakeGenericType(type.GetElementType().ToGraphType());
                
            if (type.IsConstructedGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return typeof(ListGraphType<>).MakeGenericType(type.GetGenericArguments()[0].ToGraphType());

            var isNullable = false;
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                type = underlyingType;
                isNullable = true;
            }

            var graphType = Attribute.IsDefined(type, typeof(GraphQLTypeAttribute))
                ? type.GetTypeInfo().GetCustomAttribute<GraphQLTypeAttribute>().GraphType.MakeGenericType(type)
                : type.GetGraphTypeFromType(true);

            return type.GetTypeInfo().IsValueType && !isNullable ? typeof(NonNullGraphType<>).MakeGenericType(graphType) : graphType;
        }

        /// <summary>
        /// Retrieves all the annotated types in the same namespace as the given type.
        /// All types should exist in the same namespace to prevent naming conflicts.
        /// </summary>
        public static IEnumerable<Type> GraphTypesInNamespace(this Type type)
        {
            var assembly = type.Assembly;
            return assembly.GetTypes()
                .Where(t => string.Equals(t.Namespace, type.Namespace) && (Attribute.IsDefined(t, typeof(GraphQLInterfaceAttribute)) || Attribute.IsDefined(t, typeof(GraphQLObjectAttribute))))
                .Select(t => t.GetCustomAttribute<GraphQLTypeAttribute>().GraphType.MakeGenericType(t))
                .ToList();
        }
    }
}
