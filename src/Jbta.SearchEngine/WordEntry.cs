using System;
using Jbta.SearchEngine.FileIndexing;

namespace Jbta.SearchEngine
{ 
    public class WordEntry : IComparable<WordEntry>
    {
        public WordEntry(IFileVersion fileVersion, int position, int lineNumber)
        {
            FileVersion = fileVersion;
            Position = position;
            LineNumber = lineNumber;
        }

        public IFileVersion FileVersion { get; }

        public int Position { get; }

        public int LineNumber { get; }

        public int CompareTo(WordEntry other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var fileNameComparison = string.Compare(FileVersion.Path, other.FileVersion.Path, StringComparison.InvariantCulture);
            if (fileNameComparison != 0) return fileNameComparison;
            var positionComparison = Position.CompareTo(other.Position);
            return positionComparison != 0 ? positionComparison : LineNumber.CompareTo(other.LineNumber);
        }
    }
}