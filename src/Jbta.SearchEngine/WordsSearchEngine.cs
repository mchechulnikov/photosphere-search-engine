using System.Collections.Generic;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileIndexing.Services;
using Jbta.SearchEngine.FileWatching;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine
{
    internal class WordsSearchEngine : ISearchEngine
    {
        private readonly IIndex _index;
        private readonly IFileIndexer _indexer;
        private readonly IIndexEjector _indexEjector;
        private readonly IFileWatcher _watcher;

        public WordsSearchEngine(
            EventReactor eventReactor,
            IIndex index,
            IFileIndexer indexer,
            IIndexEjector indexEjector,
            IFileWatcher watcher)
        {
            _index = index;
            _indexer = indexer;
            _indexEjector = indexEjector;
            _watcher = watcher;

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