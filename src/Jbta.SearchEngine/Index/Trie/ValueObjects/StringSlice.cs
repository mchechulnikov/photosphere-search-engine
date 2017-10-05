using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.Index.Trie.ValueObjects
{
    internal struct StringSlice : IEnumerable<char>
    {
        public StringSlice(string source) : this(source, 0, source?.Length ?? 0)
        {
        }

        public StringSlice(string source, int startIndex, int partitionLength)
        {
            Origin = source;
            StartIndex = startIndex;
            Length = Math.Min(partitionLength, Origin.Length - startIndex);
        }

        public string Origin { get; }

        public int Length { get; }

        public int StartIndex { get; }

        public char this[int index] => Origin[StartIndex + index];

        public string SubstringFromBegin => Origin.Substring(0, StartIndex + Length);

        public bool StartsWith(StringSlice slice, bool caseSensetive = true)
        {
            if (Length < slice.Length)
            {
                return false;
            }

            var that = this;
            return caseSensetive
                ? !slice.Where((t, i) => that[i] != t).Any()
                : !slice.Where((t, i) => !that[i].EqualsIgnoreCase(t)).Any();
        }

        public IEnumerator<char> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}