using System.Collections.Generic;
using System.Threading;

namespace Jbta.Indexing.Indexing
{
    public class PrefixTree
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public Node Root { get; } = new Node();

        public Node Add(char letter, bool isTerminal, Node currentNode, string filePath)
        {
            Node node;
            _lock.EnterWriteLock();
            try
            {
                if (!currentNode.Children.TryGetValue(letter, out var next))
                {
                    next = new Node
                    {
                        IsTerminal = isTerminal,
                        Files = new HashSet<string> { filePath }
                    };
                    currentNode.Children.Add(letter, next);
                }
                next.Files.Add(filePath);
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
            for (var i = 0; i < query.Length; i++)
            {
                if (!node.Children.TryGetValue(query[i], out var next))
                {
                    return false;
                }
                if (node.IsTerminal && i == query.Length - 1)
                {
                    break;
                }
                node = next;
            }
            return true;
        }

        public IEnumerable<string> Get(string query)
        {
            var node = Root;
            for (var i = 0; i < query.Length; i++)
            {
                if (!node.Children.TryGetValue(query[i], out var next))
                {
                    return null;
                }
                if (node.IsTerminal && i == query.Length - 1)
                {
                    break;
                }
                node = next;
            }
            return node.Files;
        }

        public class Node
        {
            public Dictionary<char, Node> Children { get; } = new Dictionary<char, Node>();

            public ISet<string> Files { get; set; }

            public bool IsTerminal { get; set; }
        }

        ~PrefixTree() => _lock?.Dispose();
    }
}