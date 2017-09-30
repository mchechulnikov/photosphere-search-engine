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

        public (Node<T> node, Node<T> parent) RetrieveWithParent(Node<T> startNode, string query, int position)
        {
            startNode.Lock.EnterReadLock();
            try
            {
                var node = startNode;
                var parent = node;
                while (true)
                {
                    if (position >= query.Length)
                    {
                        break;
                    }

                    var nextNode = SearchChild(node, query, position);
                    if (nextNode == null)
                    {
                        break;
                    }
                    parent = node;
                    node = nextNode;
                    position += nextNode.Key.Length;
                }
                return (node, parent);
            }
            finally
            {
                startNode.Lock.ExitReadLock();
            }
        }

        public Node<T> RetrieveIgnoreCase(Node<T> node, string query, int position)
        {
            node.Lock.EnterReadLock();
            try
            {
                throw new NotImplementedException();
            }
            finally
            {
                node.Lock.ExitReadLock();
            }
        }

        private Node<T> SearchDeep(Node<T> node, string query, int position)
        {
            var nextNode = SearchChild(node, query, position);
            return nextNode == null
                ? null
                : Retrieve(nextNode, query, position + nextNode.Key.Length);
        }

        private static Node<T> SearchChild(Node<T> node, string query, int position)
        {
            if (!node.Children.TryGetValue(query[position], out var child))
            {
                return null;
            }

            var queryPartition = new StringSlice(query, position, child.Key.Length);
            return child.Key.StartsWith(queryPartition) ? child : null;
        }

        //private (Node<T> startNode, Node<T> parent) SearchDeep(Node<T> startNode, string query, int position)
        //{
        //    var nextNode = SearchChild(startNode, query, position);
        //    if (nextNode == null)
        //    {
        //        return (null, startNode);
        //    }
        //    var searchNode = Retrieve(nextNode, query, position + nextNode.Key.Length);
        //    return (searchNode, startNode);
        //}

        //private static Node<T> SearchChild(Node<T> startNode, string query, int position)
        //{
        //    if (!startNode.Children.TryGetValue(query[position], out var child))
        //    {
        //        return null;
        //    }

        //    var queryPartition = new StringSlice(query, position, child.Key.Length);
        //    return child.Key.StartsWith(queryPartition) ? child : null;
        //}

        //public IEnumerable<T> Retrieve(string query) => Get(_rootNode, query, 0);

        //private IEnumerable<T> Get(Node<T> startNode, string query, int position)
        //{
        //    startNode.Lock.EnterReadLock();
        //    try
        //    {
        //        return position >= query.Length
        //            ? startNode.Subtree.SelectMany(n => n.Values)
        //            : SearchDeep(query, startNode, position);
        //    }
        //    finally
        //    {
        //        startNode.Lock.ExitReadLock();
        //    }
        //}

        //private IEnumerable<T> SearchDeep(string query, Node<T> startNode, int position)
        //{
        //    var nextNode = SearchChild(startNode, query, position);
        //    return nextNode == null
        //        ? Enumerable.Empty<T>()
        //        : Get(nextNode, query, position + nextNode.Key.Length);
        //}
    }
}