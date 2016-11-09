using System;
using System.Collections;

namespace Serraview.GraphQL.ResolveQL
{
    public interface INodeCollection : IEnumerable
    {
        NodeCollection<T> GetOrAddRelation<T>(string name, Func<string, NodeCollection<T>> func) where T : GraphNode;
    }
}
