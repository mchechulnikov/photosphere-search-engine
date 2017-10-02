using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.Trie;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class Index : IIndex
    {
        private readonly ITrie<WordEntry> _searchIndex;
        private readonly IDictionary<FileVersion, ISet<string>> _directIndex;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public Index()
        {
            _searchIndex = new PatriciaTrie<WordEntry>();
            _directIndex = new Dictionary<FileVersion, ISet<string>>();
        }

        public void Add(FileVersion fileVersion, IEnumerable<ParsedWord> words)
        {
            using(_lock.ForWriting())
            {
                var setOfWords = new HashSet<string>();
                _directIndex.Add(fileVersion, setOfWords);

                foreach (var word in words)
                {
                    setOfWords.Add(word.Word);
                    _searchIndex.Add(word.Word, word.Entry);
                }
            }
        }

        public void Remove(FileVersion fileVersion)
        {
            using (_lock.ForWriting())
            {
                var words = _directIndex[fileVersion];
                foreach (var word in words)
                {
                    _searchIndex.Remove(word, we => we.FileVersion == fileVersion);
                }
                _directIndex.Remove(fileVersion);
            }
        }

        public void Remove(IReadOnlyCollection<FileVersion> fileVersions)
        {
            using (_lock.ForWriting())
            {
                var words = fileVersions.SelectMany(fv => _directIndex[fv]);
                foreach (var word in words)
                {
                    _searchIndex.Remove(word, e => fileVersions.Contains(e.FileVersion));
                }

                foreach (var fileVersion in fileVersions)
                {
                    _directIndex.Remove(fileVersion);
                }
            }
        }

        public IEnumerable<WordEntry> Get(string query, bool wholeWord)
        {
            using (_lock.ForReading())
            {
                return _searchIndex
                    .Get(query?.Trim(), wholeWord)
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
}