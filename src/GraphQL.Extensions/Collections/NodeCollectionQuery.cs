using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphQL.Extensions.Collections
{
    public class NodeCollectionQuery<T> : IEnumerable<T> where T : GraphNode
    {
        private readonly Func<NodeCollection<T>> _collectionGetFunc;

        protected readonly Func<T, bool> Filter;

        private NodeCollection<T> _collection;

        private T[] _nodes;

        public NodeCollectionQuery(Func<NodeCollection<T>> collectionGetFunc, Func<T, bool> filter = null)
        {
            _collectionGetFunc = collectionGetFunc;
            Filter = filter;
        }

        private readonly object _collectionLock = new object();

        public NodeCollection<T> Collection
        {
            get
            {
                if (_collection == null)
                {
                    lock (_collectionLock)
                    {
                        if (_collection == null)
                            _collection = _collectionGetFunc();
                    }
                }

                return _collection;
            }
        } 

        public T[] Nodes
        {
            get { return _nodes ?? (_nodes = Collection.Where(n => Filter == null || Filter(n)).ToArray()); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Nodes.AsEnumerable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}