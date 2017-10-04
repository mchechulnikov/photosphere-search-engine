using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Trie.ValueObjects;

namespace Jbta.SearchEngine.Trie.Services
{
    internal class NodeRetriever<T>
    {
        private readonly Node<T> _rootNode;

        public NodeRetriever(Node<T> rootNode)
        {
            _rootNode = rootNode;
        }

        public (Node<T> node, Stack<Node<T>> nodesStack) RetrieveForReading(string query)
        {
            var position = 0;
            var nodesStack = new Stack<Node<T>>();
            var node = _rootNode;
            while (true)
            {
                node.Lock.EnterReadLock();
                nodesStack.Push(node);

                if (position >= query.Length)
                {
                    break;
                }

                var nextNode = SearchChild(node, query, position);
                node = nextNode;
                if (nextNode == null)
                {
                    break;
                }
                position += nextNode.Key.Length;
            }
            return (node, nodesStack);
        }

        public (Node<T> node, Node<T> parent, Stack<Node<T>> nodesStack) RetrieveForWriting(string query)
        {
            var position = 0;
            var nodesStack = new Stack<Node<T>>();
            var node = _rootNode;
            var parent = node;
            while (true)
            {
                node.Lock.EnterUpgradeableReadLock();
                nodesStack.Push(node);

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
            return (node, parent, nodesStack);
        }

        public IEnumerable<Node<T>> GetSubtree(Node<T> node)
        {
            yield return node;
            var subtreeNodes = node.Children.Values.SelectMany(n =>
            {
                n.Lock.EnterReadLock();
                return GetSubtree(n);
            });

            foreach (var subtreeNode in subtreeNodes)
            {
                yield return subtreeNode;
            }
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