using System;

namespace Jbta.SearchEngine.FileIndexing
{
    public interface IFileVersion
    {
        string Path { get; }

        DateTime Version { get; }
    }
}