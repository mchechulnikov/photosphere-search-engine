using System;

namespace Jbta.Indexing.Indexing.Services
{
    internal class NodeRetriever<T>
    {
        public Node<T> Retrieve(Node<T> node, string query, int position)
        {
            node.Lock.EnterReadLock();
            try
            {
                return position >= query.Length
                    ? node
                    : SearchDeep(node, query, position);
            }
            finally
            {
                node.Lock.ExitReadLock();
            }
        }

        private Node<T> SearchDeep(Node<T> node, string query, int position)
        {
            var nextNode = GetChildOrNull(node, query, position);
            return nextNode == null
                ? null
                : Retrieve(nextNode, query, position + nextNode.Key.Length);
        }

        private Node<T> GetChildOrNull(Node<T> node, string query, int position)
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

        //public IEnumerable<T> Retrieve(string query) => Get(_rootNode, query, 0);

        //private IEnumerable<T> Get(Node<T> node, string query, int position)
        //{
        //    node.Lock.EnterReadLock();
        //    try
        //    {
        //        return position >= query.Length
        //            ? node.Subtree.SelectMany(n => n.Values)
        //            : SearchDeep(query, node, position);
        //    }
        //    finally
        //    {
        //        node.Lock.ExitReadLock();
        //    }
        //}

        //private IEnumerable<T> SearchDeep(string query, Node<T> node, int position)
        //{
        //    var nextNode = GetChildOrNull(node, query, position);
        //    return nextNode == null
        //        ? Enumerable.Empty<T>()
        //        : Get(nextNode, query, position + nextNode.Key.Length);
        //}
    }
}