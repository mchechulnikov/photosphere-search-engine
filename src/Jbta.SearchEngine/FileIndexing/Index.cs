using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.Trie;
using Jbta.SearchEngine.Vendor.NonBlocking.ConcurrentDictionary;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class Index : IIndex
    {
        private readonly ITrie<WordEntry> _searchIndex;
        private readonly ConcurrentDictionary<FileVersion, ISet<string>> _directIndex;

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
                setOfWords.Add(word.Word);
                _searchIndex.Add(word.Word, word.Entry);
            }
        }

        public void Remove(IReadOnlyCollection<FileVersion> fileVersions)
        {
            var words = fileVersions.SelectMany(fv => _directIndex[fv]).Distinct().ToList();
            foreach (var word in words)
            {
                _searchIndex.Remove(word, e => fileVersions.Contains(e.FileVersion));
            }

            foreach (var fileVersion in fileVersions)
            {
                _directIndex.TryRemove(fileVersion, out var _);
            }
        }

        public IEnumerable<WordEntry> Get(string query, bool wholeWord) => _searchIndex.Get(query, wholeWord);
    }
}