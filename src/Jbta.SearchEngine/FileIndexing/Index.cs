using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.Trie;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class Index : IIndex
    {
        private readonly ITrie<WordEntry> _searchIndex;
        private readonly IDictionary<FileVersion, ISet<string>> _directIndex;

        public Index()
        {
            _searchIndex = new PatriciaTrie<WordEntry>();
            _directIndex = new Dictionary<FileVersion, ISet<string>>();
        }

        public void Add(FileVersion fileVersion, IEnumerable<ParsedWord> words)
        {
            var setOfWords = new HashSet<string>();
            _directIndex.Add(fileVersion, setOfWords);

            foreach (var word in words)
            {
                setOfWords.Add(word.Word);
                _searchIndex.Add(word.Word, word.Entry);
            }
        }

        public void Remove(FileVersion fileVersion)
        {
            var words = _directIndex[fileVersion];
            foreach (var word in words)
            {
                _searchIndex.Remove(word, we => we.FileVersion == fileVersion);
            }
            _directIndex.Remove(fileVersion);
        }

        public void Remove(IReadOnlyCollection<FileVersion> fileVersions)
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

        public IEnumerable<WordEntry> Get(string query, bool wholeWord)
        {
            return _searchIndex.Get(query?.Trim(), wholeWord);
        }
    }
}