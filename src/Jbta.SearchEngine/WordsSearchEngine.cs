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
            var eventReactor = new EventReactor();
            var fileParserProvider = new FileParserProvider(settings);
            var filesVersionsRegistry = new FilesVersionsRegistry(eventReactor);
            _index = new Index();
            _indexer = new FileIndexer(eventReactor, fileParserProvider, _index, filesVersionsRegistry, settings);
            _indexEjector = new IndexEjector(eventReactor, _index, filesVersionsRegistry, settings);
            var indexUpdater = new IndexUpdater(_indexer, _index, filesVersionsRegistry);
            _watcher = new FileWatcher(_indexer, indexUpdater, _indexEjector, filesVersionsRegistry);

            eventReactor.Register(EngineEvent.FileIndexing, a => FileIndexing?.Invoke(a));
            eventReactor.Register(EngineEvent.FileIndexed, a => FileIndexed?.Invoke(a));
            eventReactor.Register(EngineEvent.FileRemoving, a => FileRemoving?.Invoke(a));
            eventReactor.Register(EngineEvent.FileRemoved, a => FileRemoved?.Invoke(a));
            eventReactor.Register(EngineEvent.FilePathChanged, a => FilePathChanged?.Invoke(a));
        }

        public event SearchEngineEventHandler FileIndexing;
        public event SearchEngineEventHandler FileIndexed;
        public event SearchEngineEventHandler FileRemoving;
        public event SearchEngineEventHandler FileRemoved;
        public event SearchEngineEventHandler FilePathChanged;

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

        public void Dispose() => _watcher?.Dispose();
    }
}