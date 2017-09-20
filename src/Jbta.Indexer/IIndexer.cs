using System.Collections.Generic;

namespace Jbta.Indexing
{
    public interface IIndexer
    {
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
        /// <returns>All files and positions in files that contans searched word</returns>
        IEnumerable<WordMatch> Search(string query, bool isCaseSensetive, bool isWholeWord);
    }
}
