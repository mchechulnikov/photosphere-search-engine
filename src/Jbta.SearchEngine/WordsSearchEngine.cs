using System.Collections.Generic;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileWatching;
using Jbta.SearchEngine.Index;
using Jbta.SearchEngine.Searching;

namespace Jbta.SearchEngine
{
    public class WordsSearchEngine : ISearchEngine
    {
        private readonly IFileIndexer _indexer;
        private readonly IFileWatcher _watcher;
        private readonly ISearcher _searcher;

        public WordsSearchEngine()
        {
            var searchIndex = new PatriciaTrie<WordEntry>();
            var fileParser = new StandartFileParser();
            _indexer = new FileIndexer(fileParser, searchIndex);
            _watcher = new FileWatcher(_indexer);
            _searcher = new Searcher(searchIndex);
        }

        public void Add(string path)
        {
            _watcher.Watch(path);
            _indexer.Index(path);
        }

        public void Remove(string path)
        {
            _watcher.Unwatch(path);
        }

        public IEnumerable<WordEntry> Search(string query, bool wholeWord = false)
        {
            return _searcher.Search(query, wholeWord);
        }
    }
}