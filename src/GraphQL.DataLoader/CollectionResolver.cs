using System;
using System.Collections.Generic;
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
        private readonly Func<TParent,int> m_KeySelector;
        private readonly DataLoader<TChild> m_Loader;

        public CollectionResolver(
            Func<TParent,int> keySelector,
            FetchDelegate<TChild> fetch)
        {
            m_KeySelector = keySelector;
            m_Loader = new DataLoader<TChild>(fetch);
        }

        public Task<IEnumerable<TChild>> Resolve(ResolveFieldContext<TParent> context)
        {
            var key = m_KeySelector(context.Source);
            return m_Loader.LoadAsync(key);
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
}
