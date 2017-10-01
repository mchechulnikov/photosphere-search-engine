using System;
using System.Collections.Generic;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileIndexing.Services;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileWatching;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine
{
    public class WordsSearchEngine : ISearchEngine
    {
        private readonly IIndex _index;
        private readonly IFileIndexer _indexer;
        private readonly IFileWatcher _watcher;
        private readonly IIndexEjector _indexEjector;

        public WordsSearchEngine() : this(new Settings()) { }

        public WordsSearchEngine(Settings settings)
        {
            var fileParserProvider = new FileParserProvider(settings);
            var filesVersionsRegistry = new FilesVersionsRegistry();
            _index = new Index();
            _indexer = new FileIndexer(fileParserProvider, _index, filesVersionsRegistry, settings);
            _indexEjector = new IndexEjector(filesVersionsRegistry, _index, settings);
            var indexUpdater = new IndexUpdater(_indexer, _index, filesVersionsRegistry);
            _watcher = new FileWatcher(_indexer, indexUpdater, _indexEjector, filesVersionsRegistry);
        }

        public event EventHandler<FileEventArgs> FileIndexed;
        public event EventHandler<FileEventArgs> FileRemovedFromIndex;
        public event EventHandler<FileEventArgs> FilePathChanged;

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
            _indexEjector.Eject(path);
        }

        public IEnumerable<WordEntry> Search(string query, bool wholeWord = false)
        {
            return _index.Get(query, wholeWord);
        }

        public void Dispose() => _watcher?.Dispose();
    }
}