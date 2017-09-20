using System.Collections.Generic;
using System.Threading;

namespace Jbta.Indexing.Indexing
{
    public class PrefixTree
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public Node Root { get; } = new Node();

        public Node Add(char letter, bool isTerminal, Node currentNode)
        {
            Node node;
            _lock.EnterWriteLock();
            try
            {
                if (!currentNode.Children.TryGetValue(letter, out var next))
                {
                    next = new Node
                    {
                        Symbol = letter,
                        IsTerminal = isTerminal
                    };
                    currentNode.Children.Add(letter, next);
                }
                node = next;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
            return node;
        }

        public void Add(char[] word)
        {
            var node = Root;
            for (var i = 0; i < word.Length; i++)
            {
                _lock.EnterWriteLock();
                try
                {
                    var letter = word[i];
                    if (!node.Children.TryGetValue(letter, out var next))
                    {
                        next = new Node
                        {
                            Symbol = letter,
                            IsTerminal = i == word.Length - 1
                        };
                        node.Children.Add(letter, next);
                    }
                    node = next;
                }
                finally
                {
                    _lock.ExitWriteLock();
                }
            }
        }

        public bool Contains(string query)
        {
            var node = Root;
            foreach (var letter in query)
            {
                if (node.IsTerminal)
                {
                    break;
                }
                if (!node.Children.TryGetValue(letter, out var next))
                {
                    return false;
                }
                node = next;
            }
            return true;
        }

        public class Node
        {
            public char Symbol { get; set; }

            public bool IsTerminal { get; set; }

            public Dictionary<char, Node> Children { get; } = new Dictionary<char, Node>();
        }

        ~PrefixTree() => _lock?.Dispose();
    }
}