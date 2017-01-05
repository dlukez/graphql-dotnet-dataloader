using DataLoader.Core;
using GraphQL.Builders;

namespace GraphQL.DataLoader
{
    public static class GraphQLExtensions
    {
        /// <summary>
        /// Configures a field to use a <see cref="DataLoaderResolver"/> to resolve batches.
        /// </summary>
        public static FieldBuilder<TSource, IEnumerable<TReturn>> BatchResolve<TSource, TKey, TReturn>(this FieldBuilder<TSource, object> fieldBuilder, Func<TSource, TKey> keySelector, FetchDelegate<TKey, TReturn> fetchDelegate)
        {
            return fieldBuilder
                .Returns<IEnumerable<TReturn>>()
                .Resolve(new DataLoaderResolver<TSource, TKey, TReturn>(keySelector, fetchDelegate));
        }

        /// <summary>
        /// Configures a field to use a <see cref="DataLoaderResolver"/> to resolve batches.
        /// </summary>
        public static FieldBuilder<TSource, IEnumerable<TReturn>> BatchResolve<TSource, TKey, TReturn>(this FieldBuilder<TSource, object> fieldBuilder, Func<TSource, TKey> keySelector, DataLoaderResolverDelegate<TKey, TReturn> fetchDelegate)
        {
            return fieldBuilder
                .Returns<IEnumerable<TReturn>>()
                .Resolve(new DataLoaderResolver<TSource, TKey, TReturn>(keySelector, fetchDelegate));
        }
    }
}