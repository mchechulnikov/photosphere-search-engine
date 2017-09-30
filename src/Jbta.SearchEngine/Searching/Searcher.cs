using System.Collections.Generic;
using Jbta.SearchEngine.Index;

namespace Jbta.SearchEngine.Searching
{
    internal class Searcher : ISearcher
    {
        private readonly ITrie<WordEntry> _searchIndex;

        public Searcher(ITrie<WordEntry> searchIndex)
        {
            _searchIndex = searchIndex;
        }

        public IEnumerable<WordEntry> Search(string query, bool wholeWord)
        {
            return _searchIndex.Get(query, wholeWord);
        }
    }
}