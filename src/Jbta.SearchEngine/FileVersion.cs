using System;

namespace Jbta.SearchEngine
{
    internal class FileVersion : IComparable<FileVersion>, IFileVersion
    {
        public FileVersion(string path, DateTime version)
        {
            Path = path;
            Version = version;
        }

        public string Path { get; set; }

        public DateTime Version { get; }

        public int CompareTo(FileVersion other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Version.CompareTo(other.Version);
        }
    }
}