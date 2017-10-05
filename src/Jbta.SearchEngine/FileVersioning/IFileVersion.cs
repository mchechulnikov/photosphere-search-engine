using System;

namespace Jbta.SearchEngine.FileVersioning
{
    public interface IFileVersion
    {
        string Path { get; }

        bool IsDead { get; set; }

        DateTime LastWriteDate { get; }

        DateTime CreationDate { get; }
    }
}