using System;

namespace Jbta.SearchEngine
{
    public interface IFileVersion
    {
        string Path { get; }

        bool IsDead { get; set; }

        DateTime LastWriteDate { get; }

        DateTime CreationDate { get; }
    }
}