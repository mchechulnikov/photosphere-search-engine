using System;
using System.Collections.Generic;
using Jbta.SearchEngine.Index.Services;

namespace Jbta.SearchEngine.Index
{
    internal sealed class PatriciaTrie<T> : ITrie<T>
    {
        private readonly KeyAdder<T> _keyAdder;
        private readonly KeyRemover<T> _keyRemover;
        private readonly ValuesGetter<T> _valuesGetter;

        public PatriciaTrie()
        {
            var rootNode = new Node<T>();
            var nodeRetriever = new NodeRetriever<T>();
            _keyAdder = new KeyAdder<T>(rootNode);
            _keyRemover = new KeyRemover<T>(nodeRetriever, rootNode);
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

        public void Remove(string key, Func<T, bool> valueSelector)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            _keyRemover.Remove(key, valueSelector);
        }

        public IEnumerable<T> Get(string query, bool wholeWord = false)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return _valuesGetter.Get(query, wholeWord);
        }
    }
}