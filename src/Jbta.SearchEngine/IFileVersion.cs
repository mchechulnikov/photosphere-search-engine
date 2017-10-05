using System;

namespace Jbta.SearchEngine
{
    /// <summary>
    /// Specific file version in search system
    /// </summary>
    public interface IFileVersion
    {
        /// <summary>
        /// Path to file
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Indicates actual this version or not
        /// </summary>
        bool IsDead { get; set; }

        /// <summary>
        /// Last write date of file for this version
        /// </summary>
        DateTime LastWriteDate { get; }

        /// <summary>
        /// Creation date of file
        /// </summary>
        DateTime CreationDate { get; }
    }
}