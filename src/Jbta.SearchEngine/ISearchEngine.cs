using System;
using System.Collections.Generic;
using Jbta.SearchEngine.Events;

namespace Jbta.SearchEngine
{
    public interface ISearchEngine : IDisposable
    {
        /// <summary>
        /// All pathes, which were added to index
        /// </summary>
        IEnumerable<string> PathesUnderIndex { get; }

        /// <summary>
        /// Add file or directory to system
        /// </summary>
        /// <param name="path">Path to directory or file that you want to be indexed</param>
        /// <returns>Will be added or not</returns>
        bool Add(string path);

        /// <summary>
        /// Remove file or directory from system. Can be removed only path that was added.
        /// </summary>
        /// <param name="path">Path to directory or file</param>
        /// <returns>Will be removed or not</returns>
        bool Remove(string path);

        /// <summary>
        /// Get entries in indexed files
        /// </summary>
        /// <param name="query">Search query (will be trimmed)</param>
        /// <param name="wholeWord">Indicates what kind of entries will be finded: whole word match or prefix match</param>
        /// <returns></returns>
        IEnumerable<WordEntry> Search(string query, bool wholeWord = false);

        /// <summary>
        /// Get pathes to indexed files that contains query
        /// </summary>
        /// <param name="query">Search query (will be trimmed)</param>
        /// <param name="wholeWord">Indicates what kind of entries will be finded: whole word match or prefix match</param>
        /// <returns></returns>
        IEnumerable<string> SearchFiles(string query, bool wholeWord = false);

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
    }
}