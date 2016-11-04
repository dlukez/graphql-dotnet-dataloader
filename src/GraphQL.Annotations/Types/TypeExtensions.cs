using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Types;

namespace GraphQL.Annotations.Types
{
    public static class TypeExtensions
    {
        public static Type ToGraphType(this Type t)
        {
            var elemType = t.GetInterface(typeof(IEnumerable<>).Name)?.GetGenericArguments()[0];

            var typeInfo = t.GetTypeInfo();

            if (t.IsGraphType())
                return t;

            if (t.IsArray)
                return typeof(ListGraphType<>).MakeGenericType(t.GetElementType().ToGraphType());
                
            if (t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return typeof(ListGraphType<>).MakeGenericType(t.GetGenericArguments()[0].ToGraphType());

            if (typeInfo.IsValueType)
                return typeof(NonNullGraphType<>).MakeGenericType(t.ToGraphType());

            return typeInfo.GetCustomAttribute<GraphQLTypeAttribute>()?.GraphType.MakeGenericType(t) ?? t.GetGraphTypeFromType();
        }

        /// <summary>
        /// Retrieves all the annotated types in the same namespace as the given type.
        /// All types should exist in the same namespace to prevent naming conflicts.
        /// </summary>
        public static IEnumerable<Type> GraphTypesInNamespace(this Type type)
        {
            var assembly = type.Assembly;
            return assembly.GetTypes()
                .Where(t => string.Equals(t.Namespace, type.Namespace))
                .Select(t => t.GetCustomAttribute<GraphQLTypeAttribute>()?.GraphType.MakeGenericType(t))
                .Where(t => t != null)
                .ToList();
        }

        // public static Type GraphTypeFromType(this Type t)
        // {
        //     return null;

        //     var nullable = false;
        //     var isCollectionType = false;

        //     if (t.IsArray)
        //     {
        //         isCollectionType = true;
        //         t = GetGraphTypeFor(t.GetElementType());
        //     }
        //     else if (t.IsGenericType)
        //     {
        //         var elemType = t.GetGenericArguments().FirstOrDefault();

        //         if (elemType != null)
        //         {
        //             if (typeof(Task<>).MakeGenericType(elemType).IsAssignableFrom(t))
        //                 return GetGraphTypeFor(elemType);

        //             if (typeof(IEnumerable<>).MakeGenericType(elemType).IsAssignableFrom(t))
        //             {
        //                 isCollectionType = true;
        //                 t = GetGraphTypeFor(elemType);
        //             }
        //         }
        //     }

        //     var underlyingType = Nullable.GetUnderlyingType(t);
        //     if (underlyingType != null)
        //     {
        //         nullable = true;
        //         t = underlyingType;
        //     }
        //     else if (t.IsClass || t.IsInterface)
        //     {
        //         nullable = true;
        //     }

        //     Type graphType;

        //     if (!nullable)
        //         graphType = typeof(NonNullGraphType<>).MakeGenericType(graphType);

        //     if (isCollectionType)
        //         graphType = typeof(ListGraphType<>).MakeGenericType(graphType);

        //     return graphType;
        // }
    }
}
