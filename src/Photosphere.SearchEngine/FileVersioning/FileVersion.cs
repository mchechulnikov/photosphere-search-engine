using System;

namespace Photosphere.SearchEngine.FileVersioning
{
    internal class FileVersion : IComparable<FileVersion>, IFileVersion
    {
        public FileVersion(string path, DateTime lastWriteDate, DateTime creationDate)
        {
            Path = path;
            LastWriteDate = lastWriteDate;
            CreationDate = creationDate;
        }

        public string Path { get; set; }

        public bool IsDead { get; set; }

        public DateTime LastWriteDate { get; }

        public DateTime CreationDate { get; }

        public int CompareTo(FileVersion other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var lastWriteDateComparison = LastWriteDate.CompareTo(other.LastWriteDate);
            if (lastWriteDateComparison != 0) return lastWriteDateComparison;
            return CreationDate.CompareTo(other.CreationDate);
        }
    }
}