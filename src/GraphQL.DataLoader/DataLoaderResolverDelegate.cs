using System.Linq;
using GraphQL.Types;

namespace GraphQL.DataLoader
{
    public delegate Task<ILookup<TKey, TValue>> DataLoaderResolverDelegate<TKey, TValue>(IEnumerable<TKey> ids, ResolveFieldContext fieldContext);
}