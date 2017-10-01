using System.Collections.Generic;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileWatching;
using Jbta.SearchEngine.Index;
using Jbta.SearchEngine.Searching;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine
{
    public class WordsSearchEngine : ISearchEngine
    {
        private readonly IFileIndexer _indexer;
        private readonly IFileWatcher _watcher;
        private readonly ISearcher _searcher;

        public WordsSearchEngine() : this(new Settings()) { }

        public WordsSearchEngine(Settings settings)
        {
            var searchIndex = new PatriciaTrie<WordEntry>();
            var fileParserProvider = new FileParserProvider(settings);
            _indexer = new FileIndexer(fileParserProvider, searchIndex, settings);
            _watcher = new FileWatcher(_indexer);
            _searcher = new Searcher(searchIndex);
        }

        public void Add(string path)
        {
            if (!FileSystem.IsExistingPath(path))
            {
                return;
            }

            _indexer.Index(path);
            _watcher.Watch(path);
        }

        public void Remove(string path)
        {
            if (!FileSystem.IsExistingPath(path))
            {
                return;
            }

            _watcher.Unwatch(path);
        }

        public IEnumerable<WordEntry> Search(string query, bool wholeWord = false)
        {
            return _searcher.Search(query, wholeWord);
        }

        public void Dispose() => _watcher?.Dispose();
    }
}