using System.Threading;
using DataLoader.Core;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.DataLoader
{
    public class DataLoaderResolver<TSource, TKey, TReturn> : IFieldResolver<Task<IEnumerable<TReturn>>>
    {
        private readonly Func<TSource, TKey> _keySelector;
        private readonly IDataLoader<TKey, TReturn> _loader;

        private bool _captureFieldContext;
        private readonly DataLoaderContext _loaderContext;
        private AsyncLocal<ResolveFieldContext> _lastContext = new AsyncLocal<ResolveFieldContext>();

        public DataLoaderResolver(Func<TSource, TKey> keySelector, IDataLoader<TKey, TReturn> loader)
        {
            _keySelector = keySelector;
            _captureFieldContext = false;
            _loader = loader;
        }

        public DataLoaderResolver(Func<TSource, TKey> keySelector, FetchDelegate<TKey, TReturn> fetchDelegate)
        {
            _keySelector = keySelector;
            _captureFieldContext = false;
            _loaderContext = new DataLoaderContext();
            _loader = new DataLoader<TKey, TReturn>(fetchDelegate, _loaderContext);
        }

        public DataLoaderResolver(Func<TSource, TKey> keySelector, DataLoaderResolverDelegate<TKey, TReturn> resolverDelegate)
        {
            _keySelector = keySelector;
            _captureFieldContext = true;
            _loaderContext = new DataLoaderContext();
            _loader = new DataLoader<TKey, TReturn>(ids => resolverDelegate(ids, _lastContext.Value), _loaderContext);
        }

        public Task<IEnumerable<TReturn>> Resolve(ResolveFieldContext context)
        {
            var source = (TSource)context.Source;
            var key = _keySelector(source);

            if (_captureFieldContext)
                _lastContext.Value = context;

            return _loader.LoadAsync(key);
        }

        object IFieldResolver.Resolve(ResolveFieldContext context)
        {
            return Resolve(context);
        }

        public async Task ExecuteAsync()
        {
            await _loader.ExecuteAsync();
        }
    }
}