using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.Index.Trie;
using Jbta.SearchEngine.Utils.Extensions;

namespace Jbta.SearchEngine.Index
{
    internal class Index : IIndex
    {
        private readonly ITrie<WordEntry> _searchIndex;
        private readonly IDictionary<IFileVersion, ISet<string>> _directIndex;
        private readonly ReaderWriterLockSlim _lock;

        public Index()
        {
            _searchIndex = new PatriciaTrie<WordEntry>();
            _directIndex = new Dictionary<IFileVersion, ISet<string>>();
            _lock = new ReaderWriterLockSlim();
        }

        public void Add(IFileVersion fileVersion, IEnumerable<ParsedWord> words)
        {
            var setOfWords = new HashSet<string>();
            using (_lock.Exclusive())
            {
                _directIndex.Add(fileVersion, setOfWords);
            }

            foreach (var word in words)
            {
                setOfWords.Add(word.Word);
                _searchIndex.Add(word.Word, word.Entry);
            }
        }

        public void Remove(IReadOnlyCollection<FileVersion> fileVersions)
        {
            if (!fileVersions.Any())
            {
                return;
            }

            using (_lock.SharedIntentExclusive())
            {
                foreach (var fileVersion in fileVersions)
                {
                    var words = _directIndex[fileVersion].ToList();
                    foreach (var word in words)
                    {
                        _searchIndex.Remove(word, e => fileVersions.Contains(e.FileVersion));
                    }
                }

                using (_lock.Exclusive())
                {
                    foreach (var fileVersion in fileVersions)
                    {
                        _directIndex.Remove(fileVersion);
                    }
                }
            }
        }

        public IEnumerable<WordEntry> Get(string query, bool wholeWord) => _searchIndex.Get(query, wholeWord);
    }
}