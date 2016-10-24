using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GraphQL.DataLoader
{
    public class DataLoaderContext
    {
        /// <summary>
        /// Context representing the current batching operation.
        /// </summary>
        public static DataLoaderContext Current { get; private set; }

        /// <summary>
        /// Runs code within a new <see cref="DataLoaderContext"/> before executing each batch load.
        /// </summary>
        public static T Run<T>(Func<DataLoaderContext, T> func)
        {
            if (Current != null)
                throw new InvalidOperationException($"An active {nameof(DataLoaderContext)} already exists");

            var ctx = new DataLoaderContext();
            Current = ctx;

            try
            {
                var result = func(ctx);
                ctx.Flush();
                return result;
            }
            finally
            {
                Current = null;
            }
        }

        /// <summary>
        /// Runs code within a new <see cref="DataLoaderContext"/> before executing each batch load.
        /// </summary>
        public static void Run(Action<DataLoaderContext> action)
        {
            Run<object>(ctx =>
            {
                action(ctx);
                return null;
            });
        }

        /// <summary>
        /// Runs code within a new <see cref="DataLoaderContext"/> before executing each batch load.
        /// </summary>
        public static T Run<T>(Func<T> func)
        {
            return Run(ctx => func());
        }

        /// <summary>
        /// Runs code within a new <see cref="DataLoaderContext"/> before executing each batch load.
        /// </summary>
        public static void Run(Action action)
        {
            Run(ctx => action());
        }

        private readonly Queue<Task> _fetchQueue;
        private readonly ConcurrentDictionary<object, IDataLoader> _loaderCache;

        /// <summary>
        /// Creates a new DataLoaderContext.
        /// </summary>
        /// <remarks>
        /// This class defines the boundary of a batching load operation
        /// </remarks>
        public DataLoaderContext()
        {
            _fetchQueue = new Queue<Task>();
            _loaderCache = new ConcurrentDictionary<object, IDataLoader>();
        }

        /// <summary>
        /// Retrieves a data loader with the given key, creating and storing one if none is found.
        /// </summary>
        public IDataLoader<TKey, TValue> GetDataLoader<TKey, TValue>(object name,
            Func<IEnumerable<TKey>, ILookup<TKey, TValue>> fetch)
        {
            return (IDataLoader<TKey, TValue>) _loaderCache.GetOrAdd(name, _ => new DataLoader<TKey, TValue>(this, fetch));
        }

        /// <summary>
        /// Executes each registered callback sequentially until there are no more to process.
        /// </summary>
        public void Flush()
        {
            while (_fetchQueue.Count > 0)
            {
                _fetchQueue.Dequeue().RunSynchronously();
            }
        }

        /// <summary>
        /// Queues the given fetch function to fire in the next round of fetches.
        /// </summary>
        internal async Task<T> Enqueue<T>(Func<T> fetch)
        {
            if (_fetchQueue == null)
                throw new ObjectDisposedException(nameof(DataLoaderContext));

            var task = new Task<T>(fetch);
            _fetchQueue.Enqueue(task);
            return await task;
        }
    }
}