using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.Trie;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class Index : IIndex
    {
        private readonly ITrie<WordEntry> _searchIndex;
        private readonly ConcurrentDictionary<FileVersion, ISet<string>> _directIndex;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public Index()
        {
            _searchIndex = new PatriciaTrie<WordEntry>();
            _directIndex = new ConcurrentDictionary<FileVersion, ISet<string>>();
        }

        public void Add(FileVersion fileVersion, IEnumerable<ParsedWord> words)
        {
            var setOfWords = new HashSet<string>();
            _directIndex.AddOrUpdate(fileVersion, setOfWords, (k, v) => v);

            foreach (var word in words)
            {
                using (_lock.ForWriting())
                {
                    setOfWords.Add(word.Word);
                    _searchIndex.Add(word.Word, word.Entry);
                }
            }
        }

        public void Remove(IReadOnlyCollection<FileVersion> fileVersions)
        {
            var words = fileVersions.SelectMany(fv => _directIndex[fv]).Distinct().ToList();
            foreach (var word in words)
            {
                //using (_lock.ForWriting())
                //{
                    _searchIndex.Remove(word, e => fileVersions.Contains(e.FileVersion));
                //}
            }

            foreach (var fileVersion in fileVersions)
            {
                _directIndex.TryRemove(fileVersion, out var _);
            }
        }

        public IEnumerable<WordEntry> Get(string query, bool wholeWord)
        {
            IEnumerable<WordEntry> wordEntries;
            using (_lock.ForReading())
            {
                wordEntries = _searchIndex.Get(query?.Trim(), wholeWord).ToList();
            }
            return wordEntries
                .GroupBy(e => e.FileVersion.Path)
                .Select(g => new
                {
                    Path = g.Key,
                    ActualVersion = g.Max(e => e.FileVersion.Version),
                    Entry = g.ToList()
                })
                .SelectMany(o => o.Entry.Where(e => e.FileVersion.Version == o.ActualVersion));
            
        }
    }
}