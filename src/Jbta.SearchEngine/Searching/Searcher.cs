using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Index;

namespace Jbta.SearchEngine.Searching
{
    internal class Searcher : ISearcher
    {
        private readonly IIndex _index;

        public Searcher(IIndex index)
        {
            _index = index;
        }

        public IEnumerable<WordEntry> Search(string query, bool wholeWord = false)
        {
            return _index
                .Get(query?.Trim(), wholeWord)
                .Where(e => e.FileVersion != null && !e.FileVersion.IsDead);
        }

        public IEnumerable<string> SearchFiles(string query, bool wholeWord = false)
        {
            return _index
                .Get(query?.Trim(), wholeWord)
                .Where(e => e.FileVersion != null && !e.FileVersion.IsDead)
                .Select(e => e.FileVersion.Path)
                .Distinct();
        }
    }
}