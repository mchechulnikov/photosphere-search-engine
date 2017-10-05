using System;

namespace Jbta.SearchEngine
{ 
    /// <summary>
    /// Word entry in indexed file
    /// </summary>
    public class WordEntry : IComparable<WordEntry>
    {
        public WordEntry(IFileVersion fileVersion, int position, int lineNumber)
        {
            FileVersion = fileVersion;
            Position = position;
            LineNumber = lineNumber;
        }

        /// <summary>
        /// Version of file that contains this entry
        /// </summary>
        public IFileVersion FileVersion { get; }

        /// <summary>
        /// Position of entry in line
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Line number of entry
        /// </summary>
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