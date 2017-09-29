using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jbta.Indexing.Indexing
{
    internal struct StringSlice : IEnumerable<char>
    {
        public StringSlice(string source) : this(source, 0, source?.Length ?? 0)
        {
        }

        public StringSlice(string source, int startIndex, int partitionLength)
        {
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (partitionLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(partitionLength));
            }

            Origin = source ?? throw new ArgumentNullException(nameof(source));
            StartIndex = startIndex;
            Length = Math.Min(partitionLength, Origin.Length - startIndex);
        }

        public string Origin { get; }

        public int Length { get; }

        public int StartIndex { get; }

        public char this[int index] => Origin[StartIndex + index];

        public bool StartsWith(StringSlice slice)
        {
            if (Length < slice.Length)
            {
                return false;
            }

            var that = this;
            return !slice.Where((t, i) => that[i] != t).Any();
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