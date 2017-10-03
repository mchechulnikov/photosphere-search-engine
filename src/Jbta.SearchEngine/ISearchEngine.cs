using System;
using System.Collections.Generic;
using Jbta.SearchEngine.Events;

namespace Jbta.SearchEngine
{
    public interface ISearchEngine : IDisposable
    {
        /// <summary>
        /// Raises when file starts indexing
        /// </summary>
        event SearchEngineEventHandler FileIndexing;

        /// <summary>
        /// Raises when file indexing done
        /// </summary>
        event SearchEngineEventHandler FileIndexed;

        /// <summary>
        /// Raises when the file removing from index started
        /// </summary>
        event SearchEngineEventHandler FileRemoving;

        /// <summary>
        /// Raises when the file removed from index
        /// </summary>
        event SearchEngineEventHandler FileRemoved;

        /// <summary>
        /// Raises when file path was changed
        /// </summary>
        event SearchEngineEventHandler FilePathChanged;

        /// <summary>
        /// All pathes, which were added to index
        /// </summary>
        IEnumerable<string> IndexedPathes { get; }

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