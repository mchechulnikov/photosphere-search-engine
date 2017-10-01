using System;

namespace Jbta.SearchEngine
{
    public interface IFileVersion
    {
        string Path { get; }

        DateTime Version { get; }
    }
}