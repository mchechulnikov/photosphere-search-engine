using System;
using System.Collections.Generic;
using System.Linq;

namespace Jbta.Indexing.Indexing.Services
{
    internal class ValuesRetriever<T>
    {
        private readonly Node<T> _rootNode;

        public ValuesRetriever(Node<T> rootNode)
        {
            _rootNode = rootNode;
        }

        public IEnumerable<T> Retrieve(string query) => Get(query, _rootNode, 0);

        private IEnumerable<T> Get(string query, Node<T> node, int position)
        {
            node.Lock.EnterReadLock();
            try
            {
                return position >= query.Length
                    ? node.Subtree.SelectMany(n => n.Values)
                    : SearchDeep(query, node, position);
            }
            finally
            {
                node.Lock.ExitReadLock();
            }
        }

        private IEnumerable<T> SearchDeep(string query, Node<T> node, int position)
        {
            var nextNode = GetChildOrNull(query, node, position);
            return nextNode == null
                ? Enumerable.Empty<T>()
                : Get(query, nextNode, position + nextNode.Key.Length);
        }

        private Node<T> GetChildOrNull(string query, Node<T> node, int position)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (!node.Children.TryGetValue(query[position], out var child))
            {
                return null;
            }
            var queryPartition = new StringSlice(query, position, child.Key.Length);
            return child.Key.StartsWith(queryPartition) ? child : null;
        }
    }
}