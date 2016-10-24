using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Builders;
using GraphQL.Types;

namespace GraphQL.DataLoader
{
    public static class DataLoaderExtensions
    {
        public static FieldBuilder<TSource, IEnumerable<TValue>> Resolve<TSource, TKey, TValue>(this FieldBuilder<TSource, object> fieldBuilder, Func<TSource, TKey> identityFunc, Func<IEnumerable<TKey>, ILookup<TKey, TValue>> fetchFunc)
        {
            return fieldBuilder
                .Returns<IEnumerable<TValue>>()
                .Resolve(new DataLoaderResolver<TSource, TKey, TValue>(identityFunc, fetchFunc));
        }

        public static IDataLoader<int, TValue> GetDataLoader<TValue>(this ResolveFieldContext context, Func<IEnumerable<int>, ILookup<int, TValue>> fetchFunc)
        {
            return ((DataLoaderContext) context.RootValue).GetDataLoader(context.ParentType.Name + "." + context.FieldName, fetchFunc);
        }
    }
}