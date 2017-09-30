using System;
using Jbta.SearchEngine.Index.Services.Adding.ValueObjects;
using Jbta.SearchEngine.Index.ValueObjects;

namespace Jbta.SearchEngine.Index.Services
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
    }
}