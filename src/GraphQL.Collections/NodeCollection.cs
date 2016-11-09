using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Serraview.GraphQL.ResolveQL
{
    public class NodeCollection<T> : INodeCollection, IList<T> where T : GraphNode
    {
        private readonly IList<T> _nodes = new List<T>();

        private readonly ConcurrentDictionary<string, INodeCollection> _relations = new ConcurrentDictionary<string, INodeCollection>();

        public NodeCollection(IEnumerable<T> nodes = null)
        {
            if (nodes == null)
                return;

            foreach (var node in nodes)
                Add(node);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        public NodeCollection<TR> GetOrAddRelation<TR>(string name, Func<string, NodeCollection<TR>> func) where TR : GraphNode
        {
            return (NodeCollection<TR>)_relations.GetOrAdd(name, func);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_nodes).GetEnumerator();
        }

        public void Add(T item)
        {
            item.Collection = this;
            _nodes.Add(item);
        }

        public void Clear()
        {
            _nodes.Clear();
        }

        public bool Contains(T item)
        {
            return _nodes.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _nodes.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _nodes.Remove(item);
        }

        public int Count
        {
            get { return _nodes.Count; }
        }

        public bool IsReadOnly
        {
            get { return _nodes.IsReadOnly; }
        }

        public int IndexOf(T item)
        {
            return _nodes.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            item.Collection = this;
            _nodes.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _nodes.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return _nodes[index]; }
            set { _nodes[index] = value; }
        }
    }
}