using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jbta.Indexing.Indexing
{
    public struct StringSlice : IEnumerable<char>
    {
        private readonly string _source;
        private readonly int _startIndex;

        public StringSlice(string source) : this(source, 0, source?.Length ?? 0)
        {
        }

        public StringSlice(string source, int startIndex, int partitionLength)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex), "The value can not be negative");
            }
            if (partitionLength < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(partitionLength), "The value must be non negative.");
            }

            _source = string.Intern(source);
            _startIndex = startIndex;
            var availableLength = _source.Length - startIndex;
            Length = Math.Min(partitionLength, availableLength);
        }

        public int Length { get; }

        public char this[int index] => _source[_startIndex + index];

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

        public bool Equals(StringSlice other)
        {
            return string.Equals(_source, other._source) && Length == other.Length && _startIndex == other._startIndex;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is StringSlice && Equals((StringSlice)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_source != null ? _source.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Length;
                hashCode = (hashCode * 397) ^ _startIndex;
                return hashCode;
            }
        }

        public bool StartsWith(StringSlice other)
        {
            if (Length < other.Length)
            {
                return false;
            }

            var tmpThis = this;
            return !other.Where((t, i) => tmpThis[i] != t).Any();
        }

        public SplitResult Split(int splitAt)
        {
            var head = new StringSlice(_source, _startIndex, splitAt);
            var rest = new StringSlice(_source, _startIndex + splitAt, Length - splitAt);
            return new SplitResult(head, rest);
        }

        public ZipResult ZipWith(StringSlice other)
        {
            var splitIndex = 0;
            using (var thisEnumerator = GetEnumerator())
            using (var otherEnumerator = other.GetEnumerator())
            {
                while (thisEnumerator.MoveNext() && otherEnumerator.MoveNext())
                {
                    if (thisEnumerator.Current != otherEnumerator.Current)
                    {
                        break;
                    }
                    splitIndex++;
                }
            }

            var thisSplitted = Split(splitIndex);
            var otherSplitted = other.Split(splitIndex);

            var commonHead = thisSplitted.Head;
            var restThis = thisSplitted.Rest;
            var restOther = otherSplitted.Rest;
            return new ZipResult(commonHead, restThis, restOther);
        }

        public override string ToString()
        {
            var result = new string(this.ToArray());
            return string.Intern(result);
        }

        public static bool operator ==(StringSlice left, StringSlice right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StringSlice left, StringSlice right)
        {
            return !(left == right);
        }
    }
}