using System;
using System.Collections.Generic;
using Jbta.Indexing.Indexing.Services;

namespace Jbta.Indexing.Indexing
{
    internal sealed class PatriciaTrie<T> : ITrie<T>
    {
        private readonly KeyAdder<T> _keyAdder;
        private readonly KeyRemover<T> _keyRemover;
        private readonly ValuesGetter<T> _valuesGetter;

        public PatriciaTrie()
        {
            var rootNode = new Node<T>();
            var keysZipper = new KeysZipper();
            var nodeRetriever = new NodeRetriever<T>();
            _keyAdder = new KeyAdder<T>(keysZipper, rootNode);
            _keyRemover = new KeyRemover<T>(keysZipper, nodeRetriever, rootNode);
            _valuesGetter = new ValuesGetter<T>(nodeRetriever, rootNode);
        }

        public void Add(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _keyAdder.Add(key, value);
        }

        public void Remove(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _keyRemover.Remove(key);
        }

        public IEnumerable<T> Get(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return _valuesGetter.Get(query);
        }
    }
}