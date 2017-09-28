using System.Collections.Generic;
using System.Threading;

namespace Jbta.Indexing.Indexing
{
    public class PrefixTree<T>
    {
        public Node Root { get; } = new Node();

        public Node Add(char letter, Node currentNode)
        {
            if (currentNode.Children.TryGetValue(letter, out var node))
            {
                return node;
            }

            node = new Node();
            currentNode.Children.Add(letter, node);
            return node;
        }

        public bool Contains(string query)
        {
            var node = Root;
            for (var i = 0; i < query.Length; i++)
            {
                node.Lock.EnterReadLock();
                try
                {
                    if (!node.Children.TryGetValue(query[i], out var next))
                    {
                        return false;
                    }
                    if (node.Files != null && i == query.Length - 1)
                    {
                        break;
                    }
                    node = next;
                }
                finally
                {
                    node.Lock.ExitReadLock();
                }
            }
            return true;
        }

        public IEnumerable<T> Get(string query)
        {
            var node = Root;
            for (var i = 0; i < query.Length; i++)
            {
                if (!node.Children.TryGetValue(query[i], out var next))
                {
                    return null;
                }
                node = next;
            }
            //if (node.Files != null)
            //{
            //    return node.Files;
            //}
            return node.Files;
        }

        public class Node
        {
            public IDictionary<char, Node> Children { get; } = new Dictionary<char, Node>();

            public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

            public ISet<T> Files { get; set; }

            ~Node() => Lock?.Dispose();
        }

        //public void Add(char[] word, string file)
        //{
        //    var node = Root;
        //    for (var i = 0; i < word.Length; i++)
        //    {
        //        var letter = word[i];
        //        if (!node.Children.TryGetValue(letter, out var next))
        //        {
        //            next = new Node();
        //            node.Children.Add(letter, next);
        //        }
        //        if (i == word.Length - 1) // is terminal
        //        {
        //            if (next.Files == null)
        //            {
        //                node.Files = new SortedSet<string>();
        //            }
        //            node.Files.Add(file);
        //        }
        //        node = next;
        //    }
        //}

    }
}