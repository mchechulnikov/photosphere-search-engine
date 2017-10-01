using System;
using System.Collections.Generic;
using Jbta.SearchEngine.Events;

namespace Jbta.SearchEngine
{
    public interface ISearchEngine : IDisposable
    {
        event EventHandler<FileEventArgs> FileIndexed;

        event EventHandler<FileEventArgs> FileRemovedFromIndex;

        event EventHandler<FileEventArgs> FilePathChanged;

        /// <summary>
        /// Add file or directory to system
        /// </summary>
        /// <param name="path">Path to directory or file that you want to be indexed</param>
        void Add(string path);

        /// <summary>
        /// Remove file or directory from system
        /// </summary>
        /// <param name="path">Path to directory or file</param>
        void Remove(string path);

        /// <summary>
        /// Get indexed files that contains searched word
        /// </summary>
        IEnumerable<WordEntry> Search(string query, bool wholeWord = false);
    }
}