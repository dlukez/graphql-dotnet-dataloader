using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Types;
using GraphQL.Resolvers;

namespace GraphQL.DataLoader
{
    /// <summary>
    /// Collects each item into a batch that can be fetched or queried in one call.
    /// Fetch functions are queued and executed sequentially after all other fields have
    /// completed processing.
    /// </summary>
    public class CollectionResolver<TParent,TChild> : IFieldResolver<Task<IEnumerable<TChild>>>
    {
        private readonly Func<TParent,int> _keySelector;
        private DataLoader<TChild> _loader;

        public CollectionResolver(
            Func<TParent,int> keySelector,
            FetchDelegate<TChild> fetch)
        {
            _keySelector = keySelector;
            _loader = new DataLoader<TChild>(fetch);
        }

        public Task<IEnumerable<TChild>> Resolve(ResolveFieldContext<TParent> context)
        {
            var key = _keySelector(context.Source);
            return _loader.LoadAsync(key);
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

        private static void Log(string str, params object[] parts)
        {
            Console.WriteLine("{0}{1} (thread {2})", new String('\t', FetchQueue.Current.Level), string.Format(str, parts), Thread.CurrentThread.ManagedThreadId);
        }

        private static string FormatField(int key, ResolveFieldContext<TParent> context)
        {
            return string.Format("{0}[{1}].{2}", typeof(TParent).Name, key, context.FieldName);
        }
    }
}
