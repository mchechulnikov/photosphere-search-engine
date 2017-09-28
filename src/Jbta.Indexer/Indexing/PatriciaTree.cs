using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Jbta.Indexing.Indexing
{
    public sealed class PatriciaTrie<T> : ITrie<T>
    {
        private readonly PatriciaTrieNode<T> _root;

        public PatriciaTrie()
        {
            _root = new PatriciaTrieNode<T>();
        }

        public IEnumerable<T> Search(string query)
        {
            return _root.Retrieve(query, 0);
        }

        public void Add(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _root.GetOrCreateChild(new StringSlice(key), value);
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }
    }
}