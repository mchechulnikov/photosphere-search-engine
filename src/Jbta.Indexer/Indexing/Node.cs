using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Jbta.Indexing.Indexing
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

        // READING

        //public IEnumerable<T> Get(string query, int position)
        //{
        //    Lock.EnterReadLock();
        //    try
        //    {
        //        return position >= query.Length
        //            ? Subtree.SelectMany(node => node.Values)
        //            : SearchDeep(query, position);
        //    }
        //    finally
        //    {
        //        Lock.ExitReadLock();
        //    }
        //}

        //private IEnumerable<T> SearchDeep(string query, int position)
        //{
        //    var nextNode = GetChildOrNull(query, position);
        //    return nextNode != null
        //        ? nextNode.Get(query, position + nextNode.Key.Length)
        //        : Enumerable.Empty<T>();
        //}

        //private Node<T> GetChildOrNull(string query, int position)
        //{
        //    if (query == null)
        //    {
        //        throw new ArgumentNullException(nameof(query));
        //    }
        //    if (!Children.TryGetValue(query[position], out var child))
        //    {
        //        return null;
        //    }
        //    var queryPartition = new StringSlice(query, position, child.Key.Length);
        //    return child.Key.StartsWith(queryPartition) ? child : null;
        //}

        // ADDING

        //public void AddToChild(StringSlice key, T value)
        //{
        //    Lock.EnterWriteLock();
        //    try
        //    {
        //        if (Children.TryGetValue(key[0], out var childNode))
        //        {
        //            childNode.Add(key, value);
        //        }
        //        else
        //        {
        //            childNode = new Node<T>(key, value);
        //            Children.Add(key[0], childNode);
        //        }
        //    }
        //    finally
        //    {
        //        Lock.ExitWriteLock();
        //    }
        //}

        //private void Add(StringSlice keyRest, T value)
        //{
        //    var zipResult = Key.ZipWith(keyRest);

        //    switch (zipResult.MatchKind)
        //    {
        //        case MatchKind.Match:
        //            AddValue(value);
        //            break;

        //        case MatchKind.IsContained:
        //            AddToChild(zipResult.SecondTail, value);
        //            break;

        //        case MatchKind.Contains:
        //            SplitOne(zipResult, value);
        //            break;

        //        case MatchKind.Partial:
        //            SplitTwo(zipResult, value);
        //            break;
        //    }
        //}

        //private void AddValue(T value)
        //{
        //    Values.Enqueue(value);
        //}

        //private void SplitOne(ZippedSlices zippedStringSlices, T value)
        //{
        //    var leftChild = new Node<T>(zippedStringSlices.FirstTail, Values, Children);

        //    Children = new Dictionary<char, Node<T>>();
        //    Values = new Queue<T>();
        //    AddValue(value);
        //    Key = zippedStringSlices.Head;

        //    Lock.EnterWriteLock();
        //    try
        //    {
        //        Children.Add(zippedStringSlices.FirstTail[0], leftChild);
        //    }
        //    finally
        //    {
        //        Lock.ExitWriteLock();
        //    }
        //}

        //private void SplitTwo(ZippedSlices zippedStringSlices, T value)
        //{
        //    var leftChild = new Node<T>(zippedStringSlices.FirstTail, Values, Children);
        //    var rightChild = new Node<T>(zippedStringSlices.SecondTail, value);

        //    Children = new Dictionary<char, Node<T>>();
        //    Values = new Queue<T>();
        //    Key = zippedStringSlices.Head;

        //    Lock.EnterWriteLock();
        //    try
        //    {
        //        var leftKey = zippedStringSlices.FirstTail[0];
        //        Children.Add(leftKey, leftChild);
        //        var rightKey = zippedStringSlices.SecondTail[0];
        //        Children.Add(rightKey, rightChild);
        //    }
        //    finally
        //    {
        //        Lock.ExitWriteLock();
        //    }
        //}
    }
}