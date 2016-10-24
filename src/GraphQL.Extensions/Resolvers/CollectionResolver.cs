using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Extensions.Resolvers
{
    public class CollectionResolver
    {
        public static Queue<Action> Pending = new Queue<Action>();
        protected static int Level;

        public static void Trigger()
        {
            var fetchers = Pending; 
            while (fetchers.Count > 0)
            {
                if (fetchers.Count == 1)
                    Level++;

                fetchers.Dequeue().Invoke();
            }
        }
        

        protected static void Log(string str, params object[] parts)
        {
            Console.WriteLine("{0}{1} (thread {2})", '\t', string.Format(str, parts), Thread.CurrentThread.ManagedThreadId);
        }
    }

    //public class CollectionResolver<TChild> : IFieldResolver<Task<IEnumerable<TChild>>>
    //{
    //    private Func<IEnumerable<TChild>> _fetch;

    //    public CollectionResolver(Func<IEnumerable<TChild>> fetch)
    //    {
    //        _fetch = fetch;
    //    }

    //    public Task<IEnumerable<TChild>> Resolve(ResolveFieldContext context)
    //    {
    //        return Task.Run(() => _fetch());
    //    }

    //    object IFieldResolver.Resolve(ResolveFieldContext context)
    //    {
    //        return Resolve(context);
    //    }
    //}

    /// <summary>
    /// Collects each item into a batch that can be fetched or queried as a single unit.
    /// Fetch functions are queued and executed sequentially after all other fields have
    /// completed processing.
    /// </summary>
    public class CollectionResolver<TParent,TChild> : CollectionResolver, IFieldResolver<Task<IEnumerable<TChild>>>
    {
        private readonly Func<TParent,int> _keySelector;
        private readonly Func<IEnumerable<int>,ILookup<int,TChild>> _fetch;
        private Task<ILookup<int,TChild>> _wait;
        private HashSet<int> _keys;
        private CountdownEvent cde;

        public CollectionResolver(
            Func<IEnumerable<int>,ILookup<int,TChild>> fetch,
            Func<TParent,int> keySelector)
        {
            _keys = new HashSet<int>();
            _keySelector = keySelector;
            _fetch = fetch;
        }

        private Task<ILookup<int,TChild>> QueueFetch(ResolveFieldContext<TParent> context)
        {
            Log("Queuing fetcher for {0}.{1}...", typeof(TParent).Name, context.FieldName);

            var tcs = new TaskCompletionSource<ILookup<int,TChild>>();

            Pending.Enqueue(() =>
            {
                Log("----------------------", Level);
                Log("Fetching {0}s[{1}].{2}", typeof(TParent).Name, string.Join(",", _keys), context.FieldName);

                var result = _fetch(_keys);
                _wait = null;
                _keys = new HashSet<int>();
                tcs.SetResult(result);
            });
            
            return tcs.Task;
        }

        public async Task<IEnumerable<TChild>> Resolve(ResolveFieldContext<TParent> context)
        {
            if (_wait == null)
                _wait = QueueFetch(context);

            var key = _keySelector(context.Source);
            _keys.Add(key);

            Log("Deferred {0}[{1}].{2}", typeof(TParent).Name, key, context.FieldName);
            var result = await _wait.ConfigureAwait(false);
            if (result == null)
                return null;

            var list = result[key].ToArray();
            Log("Resolved {0}[{1}].{2} with {3} results", typeof(TParent).Name, key, context.FieldName, list.Length);
            return list;
        }

        public Task<IEnumerable<TChild>> Resolve(ResolveFieldContext context)
        {
            var typedContext = context as ResolveFieldContext<TParent>;
            return Resolve(typedContext ?? new ResolveFieldContext<TParent>(context));
        }

        object IFieldResolver.Resolve(ResolveFieldContext context)
        {
            return Resolve(context);
        }
    }

    public class ResolvedCollection<T> : IEnumerable<T>
    {
        private IEnumerable<T> _items;
        public ResolvedCollection(IEnumerable<T> items)
        {
            _items = items;
        }

        public Action OnCompleted { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _items)
                yield return item;
            
            if (OnCompleted != null)
                OnCompleted.Invoke();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}


