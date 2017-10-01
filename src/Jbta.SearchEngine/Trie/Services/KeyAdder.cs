using System.Collections.Generic;
using Jbta.SearchEngine.Trie.Services.Adding;
using Jbta.SearchEngine.Trie.Services.Adding.ValueObjects;
using Jbta.SearchEngine.Trie.ValueObjects;

namespace Jbta.SearchEngine.Trie.Services
{
    internal class KeyAdder<T>
    {
        private readonly KeysZipper _keysZipper;
        private readonly Node<T> _rootNode;

        public KeyAdder(Node<T> rootNode)
        {
            _keysZipper = new KeysZipper();
            _rootNode = rootNode;
        }

        public void Add(string key, T value)
        {
            Add(_rootNode, new StringSlice(key), value);
        }

        private void Add(Node<T> node, StringSlice key, T value)
        {
            node.Lock.EnterWriteLock();
            try
            {
                if (node.Children.TryGetValue(key[0], out var childNode))
                {
                    AddWithZip(childNode, key, value);
                }
                else
                {
                    node.Children.Add(key[0], new Node<T>(key, value));
                }
            }
            finally
            {
                node.Lock.ExitWriteLock();
            }
        }

        private void AddWithZip(Node<T> node, StringSlice keyRest, T value)
        {
            var zippedSlices = _keysZipper.Zip(node.Key, keyRest);
            switch (zippedSlices.MatchKind)
            {
                case ZippedSlices.Match.Match:
                    node.Values.Add(value);
                    break;
                case ZippedSlices.Match.IsContained:
                    Add(node, zippedSlices.SecondTail, value);
                    break;
                case ZippedSlices.Match.Contains:
                    AddFirstTail(node, zippedSlices, value);
                    break;
                case ZippedSlices.Match.Partial:
                    AddBothTails(node, zippedSlices, value);
                    break;
            }
        }

        private static void AddFirstTail(Node<T> node, ZippedSlices zippedSlices, T value)
        {
            var leftChild = new Node<T>(zippedSlices.FirstTail, node.Values, node.Children);

            node.Lock.EnterWriteLock();
            try
            {
                node.Values = new List<T> {value};
                node.Key = zippedSlices.Head;
                node.Children = new Dictionary<char, Node<T>>
                {
                    { zippedSlices.FirstTail[0], leftChild }
                };
            }
            finally
            {
                node.Lock.ExitWriteLock();
            }
        }

        private static void AddBothTails(Node<T> node, ZippedSlices zippedSlices, T value)
        {
            node.Lock.EnterWriteLock();
            try
            {
                node.Children = new Dictionary<char, Node<T>>
                {
                    { zippedSlices.FirstTail[0], new Node<T>(zippedSlices.FirstTail, node.Values, node.Children) },
                    { zippedSlices.SecondTail[0], new Node<T>(zippedSlices.SecondTail, value) }
                };
                node.Values = new List<T>();
                node.Key = zippedSlices.Head;
            }
            finally
            {
                node.Lock.ExitWriteLock();
            }
        }
    }
}