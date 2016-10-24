using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.DataLoader
{
    public class DataLoaderResolver<TSource, TValue> : DataLoaderResolver<TSource, int, TValue>
    {
        public DataLoaderResolver(Func<TSource, int> keySelector, IDataLoader<int, TValue> loader) : base(keySelector, loader)
        {
        }

        public DataLoaderResolver(Func<TSource, int> keySelector, Func<IEnumerable<int>, ILookup<int, TValue>> fetch) : base(keySelector, fetch)
        {
        }
    }

    /// <summary>
    /// Collect the key for each source item so that they may be processed as a batch.
    /// </summary>
    public class DataLoaderResolver<TSource, TKey, TValue> : IFieldResolver<Task<IEnumerable<TValue>>>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IDataLoader<TKey, TValue> _loader;

        public DataLoaderResolver(Func<TSource, TKey> keySelector, IDataLoader<TKey, TValue> loader)
        {
            _keySelector = keySelector;
            _loader = loader;
        }

        public DataLoaderResolver(Func<TSource, TKey> keySelector, Func<IEnumerable<TKey>, ILookup<TKey, TValue>> fetch)
            : this(keySelector, new DataLoader<TKey, TValue>(fetch))
        {
        }

        public Task<IEnumerable<TValue>> Resolve(ResolveFieldContext context)
        {
            var source = (TSource)context.Source;
            var key = _keySelector(source);
            return _loader.LoadAsync(key);
        }

        object IFieldResolver.Resolve(ResolveFieldContext context)
        {
            return Resolve(context);
        }
    }
}
