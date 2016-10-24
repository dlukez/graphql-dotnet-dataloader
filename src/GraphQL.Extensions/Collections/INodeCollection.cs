using System;
using System.Collections;

namespace GraphQL.Extensions.Collections
{
    public interface INodeCollection : IEnumerable
    {
        NodeCollection<T> GetOrAddRelation<T>(string name, Func<string, NodeCollection<T>> func) where T : GraphNode;
    }
}
