using System;

namespace Jbta.Indexing
{ 
    public class WordEntry : IComparable<WordEntry>
    {
        public WordEntry(string fileName, int position, int lineNumber)
        {
            FileName = fileName;
            Position = position;
            LineNumber = lineNumber;
        }

        public string FileName { get; }

        public int Position { get; }

        public int LineNumber { get; }

        public int CompareTo(WordEntry other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var fileNameComparison = string.Compare(FileName, other.FileName, StringComparison.InvariantCulture);
            if (fileNameComparison != 0) return fileNameComparison;
            var positionComparison = Position.CompareTo(other.Position);
            return positionComparison != 0 ? positionComparison : LineNumber.CompareTo(other.LineNumber);
        }
    }
}