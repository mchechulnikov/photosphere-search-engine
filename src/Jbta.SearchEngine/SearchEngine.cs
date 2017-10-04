using System.Collections.Generic;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileIndexing.Services;
using Jbta.SearchEngine.FileSupervision;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine
{
    internal class SearchEngine : ISearchEngine
    {
        private readonly IIndex _index;
        private readonly IFileIndexer _indexer;
        private readonly IIndexEjector _indexEjector;
        private readonly IFileSupervisor _supervisor;

        public SearchEngine(
            EventReactor eventReactor,
            IIndex index,
            IFileIndexer indexer,
            IIndexEjector indexEjector,
            IFileSupervisor supervisor)
        {
            _index = index;
            _indexer = indexer;
            _indexEjector = indexEjector;
            _supervisor = supervisor;

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

        public IEnumerable<string> IndexedPathes => _supervisor.WatchedPathes;

        public void Add(string path)
        {
            if (!FileSystem.IsExistingPath(path))
            {
                return;
            }

            _indexer.Index(path);
            _supervisor.Watch(path);
        }

        public void Remove(string path)
        {
            if (!FileSystem.IsExistingPath(path))
            {
                return;
            }

            _supervisor.Unwatch(path);
            _indexEjector.Eject(path);
        }

        public IEnumerable<WordEntry> Search(string query, bool wholeWord = false)
        {
            return _index.Get(query, wholeWord);
        }

        public void Dispose() => _supervisor?.Dispose();
    }
}