using System;
using System.Collections.Generic;

namespace Jbta.Indexing
{
    public interface IIndexer
    {
        event EventHandler IndexingStarted;
        event EventHandler IndexingStoped;

        /// <summary>
        /// Add file or directory to index
        /// </summary>
        /// <param name="path">Path to directory or file that you want to be indexed</param>
        void Add(string path);

        /// <summary>
        /// Remove file or directory from index
        /// </summary>
        /// <param name="path">Path to directory or file</param>
        void Remove(string path);

        /// <summary>
        /// Search indexed files that contains searched word
        /// </summary>
        IEnumerable<WordEntry> Search(string query, bool isCaseSensetive = false, bool isWholeWord = false);
    }
}
