using System.Collections.Generic;
using System.Threading;
using Photosphere.SearchEngine.Index.Trie.ValueObjects;
using Photosphere.SearchEngine.Vendor.NonBlocking.ConcurrentDictionary;

namespace Photosphere.SearchEngine.Index.Trie
{
    internal class Node<T>
    {
        public Node()
        {
            Key = new StringSlice(string.Empty);
            Values = new HashSet<T>();
            Children = new ConcurrentDictionary<char, Node<T>>();
        }

        public Node(StringSlice key, T value)
            : this(key, new HashSet<T>(new[] { value }), new ConcurrentDictionary<char, Node<T>>())
        {
        }

        public Node(StringSlice key, ISet<T> values, ConcurrentDictionary<char, Node<T>> children)
        {
            Values = values;
            Key = key;
            Children = children;
        }

        public ReaderWriterLockSlim Lock { get; } = new ReaderWriterLockSlim();

        public ConcurrentDictionary<char, Node<T>> Children { get; set; }

        public StringSlice Key { get; set; }

        public ISet<T> Values { get; set; }
    }
}