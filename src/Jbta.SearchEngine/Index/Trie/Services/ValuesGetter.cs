using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Utils.Extensions;

namespace Jbta.SearchEngine.Index.Trie.Services
{
    internal class ValuesGetter<T>
    {
        private readonly NodeRetriever<T> _nodeRetriever;        

        public ValuesGetter(NodeRetriever<T> nodeRetriever)
        {
            _nodeRetriever = nodeRetriever;
        }

        public IReadOnlyCollection<T> Get(string query, bool wholeWord)
        {
            var (node, nodesStack) = _nodeRetriever.RetrieveForReading(query);
            var result = GetFromNode(node, query, wholeWord);
            ReleaseLocks(nodesStack);
            return result;
        }

        private IReadOnlyCollection<T> GetFromNode(Node<T> node, string query, bool wholeWord)
        {
            if (node == null)
            {
                return new List<T>();
            }

            if (wholeWord)
            {
                return node.Key.SubstringFromBegin.Equals(query)
                    ? node.Values.NotNull().ToList()
                    : new List<T>();
            }

            return GetSubtreeValues(node);
        }

        private IReadOnlyCollection<T> GetSubtreeValues(Node<T> node)
        {
            var subtreeNodes = _nodeRetriever.GetSubtree(node).ToList();
            var subtreeValues = subtreeNodes.SelectMany(n => n.Values).NotNull().ToList();
            foreach (var subtreeNode in subtreeNodes)
            {
                var locker = subtreeNode.Lock;
                if (locker.IsReadLockHeld) locker.ExitReadLock();
            }
            return subtreeValues;
        }

        private static void ReleaseLocks(Stack<Node<T>> nodesStack)
        {
            while (nodesStack.Count > 0)
            {
                var locker = nodesStack.Pop().Lock;
                if (locker.IsReadLockHeld) locker.ExitReadLock();
            }
        }
    }
}