using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.Trie.ValueObjects;
using Jbta.SearchEngine.Vendor.NonBlocking.ConcurrentDictionary;

namespace Jbta.SearchEngine.Trie
{
    internal class Node<T>
    {
        public Node()
        {
            Key = new StringSlice(string.Empty);
            Values = new List<T>();
            Children = new ConcurrentDictionary<char, Node<T>>();
        }

        public Node(StringSlice key, T value)
            : this(key, new List<T>(new[] { value }), new ConcurrentDictionary<char, Node<T>>())
        {
        }

        public Node(StringSlice key, IList<T> values, ConcurrentDictionary<char, Node<T>> children)
        {
            Values = values;
            Key = key;
            Children = children;
        }

        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

        public ConcurrentDictionary<char, Node<T>> Children { get; set; }

        public StringSlice Key { get; set; }

        public IList<T> Values { get; set; }

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