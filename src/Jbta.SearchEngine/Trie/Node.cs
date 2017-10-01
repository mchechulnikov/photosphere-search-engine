using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.Trie.ValueObjects;

namespace Jbta.SearchEngine.Trie
{
    internal class Node<T>
    {
        public Node()
        {
            Key = new StringSlice(string.Empty);
            Values = new List<T>();
            Children = new Dictionary<char, Node<T>>();
            Lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public Node(StringSlice key, T value)
            : this(key, new List<T>(new[] { value }), new Dictionary<char, Node<T>>())
        {
        }

        public Node(StringSlice key, IList<T> values, Dictionary<char, Node<T>> children)
        {
            Values = values;
            Key = key;
            Children = children;
            Lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        public Dictionary<char, Node<T>> Children { get; set; }

        public StringSlice Key { get; set; }

        public IList<T> Values { get; set; }

        public ReaderWriterLockSlim Lock { get; }

        public IEnumerable<Node<T>> Subtree
        {
            get
            {
                yield return this;
                foreach (var node in Children.Values.SelectMany(n => n.Subtree))
                {
                    yield return node;
                }
            }
        }
    }
}