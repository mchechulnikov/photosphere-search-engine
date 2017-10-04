using System;

namespace Jbta.SearchEngine.FileVersioning
{
    public interface IFileVersion
    {
        string Path { get; }

        DateTime Version { get; }
    }
}