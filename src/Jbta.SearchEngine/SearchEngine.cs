using System.Collections.Generic;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing.Services;
using Jbta.SearchEngine.FileSupervision;
using Jbta.SearchEngine.Searching;
using Jbta.SearchEngine.Shedulling;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine
{
    internal class SearchEngine : ISearchEngine
    {
        private readonly IFileIndexer _indexer;
        private readonly IIndexEjector _indexEjector;
        private readonly IFileSupervisor _supervisor;
        private readonly ISearcher _searcher;
        private readonly ISheduller _sheduller;

        public SearchEngine(
            EventReactor eventReactor,
            IFileIndexer indexer,
            IIndexEjector indexEjector,
            IFileSupervisor supervisor,
            ISearcher searcher,
            ISheduller sheduller)
        {
            _indexer = indexer;
            _indexEjector = indexEjector;
            _supervisor = supervisor;
            _searcher = searcher;
            _sheduller = sheduller;

            eventReactor.Register(EngineEvent.FileIndexing, a => FileIndexing?.Invoke(a));
            eventReactor.Register(EngineEvent.FileIndexed, a => FileIndexed?.Invoke(a));
            eventReactor.Register(EngineEvent.FileRemoving, a => FileRemoving?.Invoke(a));
            eventReactor.Register(EngineEvent.FileRemoved, a => FileRemoved?.Invoke(a));
            eventReactor.Register(EngineEvent.FilePathChanged, a => FilePathChanged?.Invoke(a));

            _sheduller.Start();
        }

        public event SearchEngineEventHandler FileIndexing;
        public event SearchEngineEventHandler FileIndexed;
        public event SearchEngineEventHandler FileRemoving;
        public event SearchEngineEventHandler FileRemoved;
        public event SearchEngineEventHandler FilePathChanged;

        public IEnumerable<string> PathesUnderIndex => _supervisor.WatchedPathes;

        public bool Add(string path)
        {
            if (_supervisor.IsUnderWatching(path))
            {
                return false;
            }
            if (!FileSystem.IsExistingPath(path))
            {
                return false;
            }

            _indexer.Index(path);
            _supervisor.Watch(path);
            return true;
        }

        public bool Remove(string path)
        {
            if (!_supervisor.IsUnderWatching(path))
            {
                return false;
            }
            if (!FileSystem.IsExistingPath(path))
            {
                return false;
            }

            _supervisor.Unwatch(path);
            _indexEjector.Eject(path);
            return true;
        }

        public IEnumerable<WordEntry> Search(string query, bool wholeWord = false)
        {
            return _searcher.Search(query, wholeWord);
        }

        public void Dispose()
        {
            _supervisor?.Dispose();
            _sheduller?.Dispose();
        }

        ~SearchEngine()
        {
            Dispose();
        }
    }
}