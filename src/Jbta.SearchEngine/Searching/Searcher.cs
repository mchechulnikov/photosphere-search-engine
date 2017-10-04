using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.FileIndexing;

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
                .Where(e => e.FileVersion != null)
                .GroupBy(e => e.FileVersion.Path)
                .Select(g => new
                {
                    Path = g.Key,
                    ActualVersion = g.Max(e => e.FileVersion.Version),
                    Entry = g
                })
                .SelectMany(a => a.Entry.Where(e => e.FileVersion.Version == a.ActualVersion));
        }
    }
}