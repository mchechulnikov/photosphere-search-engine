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
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly IFileIndexer _indexer;
        private readonly IFileWatcher _watcher;
        private readonly IIndexEjector _indexEjector;

        public WordsSearchEngine() : this(new Settings()) { }

        public WordsSearchEngine(Settings settings)
        {
            var fileParserProvider = new FileParserProvider(settings);
            _filesVersionsRegistry = new FilesVersionsRegistry();
            _index = new Index();
            _indexer = new FileIndexer(fileParserProvider, _index, _filesVersionsRegistry, settings);
            _indexEjector = new IndexEjector(_index, _filesVersionsRegistry, settings);
            var indexUpdater = new IndexUpdater(_indexer, _index, _filesVersionsRegistry);
            _watcher = new FileWatcher(_indexer, indexUpdater, _indexEjector, _filesVersionsRegistry);

            _indexer.FileIndexingStarted += OnFileIndexingStarted;
            _indexer.FileIndexed += OnFileIndexed;
            _indexEjector.FileRemovedFromIndex += OnFileRemovedFromIndex;
            _filesVersionsRegistry.FilePathChanged += OnFilePathChanged;
        }

        public event FileIndexingEventHandler FileIndexingStarted;
        public event FileIndexingEventHandler FileIndexed;
        public event FileIndexingEventHandler FileRemovedFromIndex;
        public event FileIndexingEventHandler FilePathChanged;

        public IEnumerable<string> IndexedPathes => _watcher.WatchedPathes;

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

        public void Dispose()
        {
            _watcher?.Dispose();

            _indexer.FileIndexingStarted -= OnFileIndexingStarted;
            _indexer.FileIndexed -= OnFileIndexed;
            _indexEjector.FileRemovedFromIndex -= OnFileRemovedFromIndex;
            _filesVersionsRegistry.FilePathChanged -= OnFilePathChanged;

            FileIndexingStarted = null;
            FileIndexed = null;
            FileRemovedFromIndex = null;
            FilePathChanged = null;
        }

        private void OnFileIndexingStarted(FileIndexingEventArgs args) => FileIndexingStarted?.Invoke(args);

        private void OnFileIndexed(FileIndexingEventArgs args) => FileIndexed?.Invoke(args);

        private void OnFileRemovedFromIndex(FileIndexingEventArgs args) => FileRemovedFromIndex?.Invoke(args);

        private void OnFilePathChanged(FileIndexingEventArgs args) => FilePathChanged?.Invoke(args);
    }
}