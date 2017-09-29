using System;
using System.Collections.Generic;
using System.Linq;
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
            return node.Files;
        }

        public class Node
        {
            public IDictionary<char, Node> Children { get; } = new Dictionary<char, Node>();

            public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

            public ISet<T> Files { get; set; }

            ~Node() => Lock?.Dispose();
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;

//namespace Jbta.Indexing.Indexing
//{
//    public class PrefixTree<T>
//    {
//        public Node Root { get; } = new Node();

//        public Node Add(char letter, Node currentNode)
//        {
//            var node = currentNode.Children.FirstOrDefault(c => c.Character == letter);
//            if (node != null)
//            {
//                return node;
//            }

//            node = new Node(letter);
//            currentNode.Children.Add(node);
//            return node;

//            //if (currentNode.Children.TryGetValue(letter, out var node))
//            //{
//            //    return node;
//            //}

//            //node = new Node();
//            //currentNode.Children.Add(letter, node);
//            //return node;
//        }

//        public bool Contains(string query)
//        {
//            var node = Root;
//            for (var i = 0; i < query.Length; i++)
//            {
//                node.Lock.EnterReadLock();
//                try
//                {
//                    var next = node.Children.FirstOrDefault(c => c.Character == query[i]);
//                    if (next == null)
//                    {
//                        return false;
//                    }
//                    node = next;

//                    //if (!node.Children.TryGetValue(query[i], out var next))
//                    //{
//                    //    return false;
//                    //}
//                    //if (node.Files != null && i == query.Length - 1)
//                    //{
//                    //    break;
//                    //}
//                    //node = next;
//                }
//                finally
//                {
//                    node.Lock.ExitReadLock();
//                }
//            }
//            return true;
//        }

//        public IEnumerable<T> Get(string query)
//        {
//            var node = Root;
//            for (var i = 0; i < query.Length; i++)
//            {
//                var next = node.Children.FirstOrDefault(c => c.Character == query[i]);
//                if (next == null)
//                {
//                    return null;
//                }
//                node = next;

//                //if (!node.Children.TryGetValue(query[i], out var next))
//                //{
//                //    return null;
//                //}
//                //node = next;
//            }
//            return node.Files;
//        }

//        public class Node : IComparable<Node>
//        {
//            public Node() {}

//            public Node(char character)
//            {
//                Character = character;
//            }

//            //public IDictionary<char, Node> Children { get; } = new Dictionary<char, Node>();
//            public ISet<Node> Children { get; } = new SortedSet<Node>();

//            public char Character { get; }

//            public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

//            public ISet<T> Files { get; set; }

//            ~Node() => Lock?.Dispose();

//            public int CompareTo(Node other)
//            {
//                if (ReferenceEquals(this, other)) return 0;
//                if (ReferenceEquals(null, other)) return 1;
//                return Character.CompareTo(other.Character);
//            }
//        }
//    }
//}