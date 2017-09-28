using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Jbta.Indexing.Indexing
{
    internal class PatriciaTrieNode<T>
    {
        private ConcurrentDictionary<char, PatriciaTrieNode<T>> _сhildren;
        private StringSlice _key;
        private Queue<T> _values;

        public PatriciaTrieNode()
        {
            _key = new StringSlice(string.Empty);
            _values = new Queue<T>();
            _сhildren = new ConcurrentDictionary<char, PatriciaTrieNode<T>>();
        }

        private PatriciaTrieNode(StringSlice key, T value)
            : this(key, new Queue<T>(new[] { value }), new ConcurrentDictionary<char, PatriciaTrieNode<T>>())
        {
        }

        private PatriciaTrieNode(StringSlice key, Queue<T> values, ConcurrentDictionary<char, PatriciaTrieNode<T>> children)
        {
            _values = values;
            _key = key;
            _сhildren = children;
        }

        private int KeyLength => _key.Length;

        private IEnumerable<T> Values => _values;

        private IEnumerable<PatriciaTrieNode<T>> Children => _сhildren.Values;

        private IEnumerable<PatriciaTrieNode<T>> Subtree =>
            Enumerable.Repeat(this, 1).Concat(Children.SelectMany(child => child.Subtree));

        public void GetOrCreateChild(StringSlice key, T value)
        {
            if (!_сhildren.TryGetValue(key[0], out var child))
            {
                child = new PatriciaTrieNode<T>(key, value);
                _сhildren.AddOrUpdate(key[0], child, (c, node) => node);
            }
            else
            {
                child.Add(key, value);
            }
        }

        public IEnumerable<T> Retrieve(string query, int position)
        {
            return position >= query.Length
                ? Subtree.SelectMany(node => node.Values)
                : SearchDeep(query, position);
        }

        private void AddValue(T value)
        {
            _values.Enqueue(value);
        }

        private void Add(StringSlice keyRest, T value)
        {
            var zipResult = _key.ZipWith(keyRest);

            switch (zipResult.MatchKind)
            {
                case MatchKind.ExactMatch:
                    AddValue(value);
                    break;

                case MatchKind.IsContained:
                    GetOrCreateChild(zipResult.OtherRest, value);
                    break;

                case MatchKind.Contains:
                    SplitOne(zipResult, value);
                    break;

                case MatchKind.Partial:
                    SplitTwo(zipResult, value);
                    break;
            }
        }

        private void SplitOne(ZipResult zipResult, T value)
        {
            var leftChild = new PatriciaTrieNode<T>(zipResult.ThisRest, _values, _сhildren);

            _сhildren = new ConcurrentDictionary<char, PatriciaTrieNode<T>>();
            _values = new Queue<T>();
            AddValue(value);
            _key = zipResult.CommonHead;

            _сhildren.AddOrUpdate(zipResult.ThisRest[0], leftChild, (c, node) => node);
        }

        private void SplitTwo(ZipResult zipResult, T value)
        {
            var leftChild = new PatriciaTrieNode<T>(zipResult.ThisRest, _values, _сhildren);
            var rightChild = new PatriciaTrieNode<T>(zipResult.OtherRest, value);

            _сhildren = new ConcurrentDictionary<char, PatriciaTrieNode<T>>();
            _values = new Queue<T>();
            _key = zipResult.CommonHead;

            var leftKey = zipResult.ThisRest[0];
            _сhildren.AddOrUpdate(leftKey, leftChild, (c, node) => node);
            var rightKey = zipResult.OtherRest[0];
            _сhildren.AddOrUpdate(rightKey, rightChild, (c, node) => node);
        }

        private PatriciaTrieNode<T> GetChildOrNull(string query, int position)
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            if (_сhildren.TryGetValue(query[position], out var child))
            {
                var queryPartition = new StringSlice(query, position, child._key.Length);
                if (child._key.StartsWith(queryPartition))
                {
                    return child;
                }
            }
            return null;
        }

        private IEnumerable<T> SearchDeep(string query, int position)
        {
            var nextNode = GetChildOrNull(query, position);
            return nextNode != null
                ? nextNode.Retrieve(query, position + nextNode.KeyLength)
                : Enumerable.Empty<T>();
        }

        //public override string ToString()
        //{
        //    return $"Key: {_key}, Values: {Values().Count()} Children:{string.Join(";", _сhildren.Keys)}, ";
        //}

        //protected TrieNodeBase<T> GetOrCreateChild(char key)
        //{
        //    throw new NotSupportedException("Use alternative signature instead.");
        //}

        //public string Traversal()
        //{
        //    var result = new StringBuilder();
        //    result.Append(_key);

        //    var subtreeResult = string.Join(" ; ", _сhildren.Values.Select(node => node.Traversal()).ToArray());
        //    if (subtreeResult.Length != 0)
        //    {
        //        result.Append("[");
        //        result.Append(subtreeResult);
        //        result.Append("]");
        //    }

        //    return result.ToString();
        //}
    }
}