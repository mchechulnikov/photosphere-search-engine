using System;
using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Trie.ValueObjects;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.Trie.Services
{
    internal class KeyRemover<T>
    {
        private readonly NodeRetriever<T> _nodeRetriever;
        private readonly Node<T> _rootNode;

        public KeyRemover(
            NodeRetriever<T> nodeRetriever,
            Node<T> rootNode)
        {
            _nodeRetriever = nodeRetriever;
            _rootNode = rootNode;
        }

        public void Remove(string key, Func<T, bool> valueSelector)
        {
            var (node, parent, nodesStack) = _nodeRetriever.RetrieveForWriting(key);

            if (node == _rootNode || node == parent)
            {
                ReleaseLocks(nodesStack);
                return;
            }

            node.Lock.EnterWriteLock();

            RemoveValue(valueSelector, node);

            if (node.Values.Count > 0)
            {
                ReleaseLocks(nodesStack);
                return;
            }

            RemoveNode(node, parent);
            ReleaseLocks(nodesStack);
        }

        private static void RemoveValue(Func<T, bool> valueSelector, Node<T> node)
        {
            foreach (var value in node.Values.Where(e => e != null && valueSelector(e)).ToList())
            {
                node.Values.Remove(value);
            }
        }

        private void RemoveNode(Node<T> node, Node<T> parent)
        {
            if (node.Children.Count < 1)
            {
                parent.Children.TryRemove(node.Key[0], out var _);
                if (parent.Children.Count == 1 && !parent.Values.Any() && parent != _rootNode)
                {
                    using (parent.Lock.Write())
                    {
                        MergeParentWithAloneChild(parent);
                    }
                }
            }
            else if (node.Children.Count == 1)
            {
                MergeParentWithAloneChild(node);
            }
        }

        private static void MergeParentWithAloneChild(Node<T> parent)
        {
            var child = parent.Children.Values.First();
            parent.Key = new StringSlice(child.Key.Origin, parent.Key.StartIndex, parent.Key.Length + child.Key.Length);
            parent.Values = child.Values;
            parent.Children.Remove(child.Key[0]);
        }

        private static void ReleaseLocks(Stack<Node<T>> nodesStack)
        {
            while (nodesStack.Count > 0)
            {
                var locker = nodesStack.Pop().Lock;
                if (locker.IsWriteLockHeld) locker.ExitWriteLock();
                if (locker.IsUpgradeableReadLockHeld) locker.ExitUpgradeableReadLock();
            }
        }
    }
}