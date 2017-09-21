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
                if (!currentNode.Children.TryGetValue(letter, out node))
                {
                    node = new Node { IsTerminal = isTerminal };
                    currentNode.Children.Add(letter, node);
                }
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

        //public class FileAndPosition
        //{
        //    public FileAndPosition(string fileName, int lineNumber, int position)
        //    {
        //        FileName = fileName;
        //        Position = new PositionInFile(lineNumber, position);
        //    }

        //    public string FileName { get; }

        //    public PositionInFile Position { get; }

        //    public struct PositionInFile
        //    {
        //        public PositionInFile(int lineNumber, int position)
        //        {
        //            LineNumber = lineNumber;
        //            Position = position;
        //        }

        //        public int LineNumber { get; }

        //        public int Position { get; }
        //    }
        //}

        ~PrefixTree() => _lock?.Dispose();
    }
}