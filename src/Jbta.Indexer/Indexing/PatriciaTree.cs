using System;
using System.Collections.Generic;
using Jbta.Indexing.Indexing.Services;

namespace Jbta.Indexing.Indexing
{
    internal sealed class PatriciaTrie<T> : ITrie<T>
    {
        private readonly KeyAdder<T> _keyAdder;
        private readonly ValuesRetriever<T> _valuesRetriever;

        public PatriciaTrie()
        {
            var rootNode = new Node<T>();
            var keysZipper = new KeysZipper();
            _keyAdder = new KeyAdder<T>(keysZipper, rootNode);
            _valuesRetriever = new ValuesRetriever<T>(rootNode);
        }

        public void Add(string key, T value)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            _keyAdder.Add(key, value);
            //_root.AddToChild(new StringSlice(key), value);
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Get(string query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }

            return _valuesRetriever.Retrieve(query);
        }
    }
}