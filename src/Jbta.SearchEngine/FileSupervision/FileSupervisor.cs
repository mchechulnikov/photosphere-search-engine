using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Events;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class FileSupervisor : IFileSupervisor
    {
        private readonly IEventReactor _eventReactor;
        private readonly PathWatcherFactory _pathWatcherFactory;
        private readonly WatchersCollection _watchers;

        public FileSupervisor(
            IEventReactor eventReactor,
            PathWatcherFactory pathWatcherFactory,
            WatchersCollection watchers)
        {
            _eventReactor = eventReactor;
            _pathWatcherFactory = pathWatcherFactory;
            _watchers = watchers;
        }

        public IEnumerable<string> WatchedPathes => _watchers.Pathes;

        public void Watch(string path)
        {
            var pathWatcher = _pathWatcherFactory.New(path);
            _watchers.Add(path, pathWatcher);
            pathWatcher.Enable();

            _eventReactor.React(EngineEvent.PathWatchingStarted, path);
        }

        public void Unwatch(string path)
        {
            var watchersSquad = _watchers.Get(path);
            if (watchersSquad == null)
            {
                return;
            }

            watchersSquad.Dispose();
            _watchers.Remove(path);

            _eventReactor.React(EngineEvent.PathWatchingEnded, path);
        }

        public bool IsUnderWatching(string path)
        {
            return _watchers.Contains(path) || _watchers.Pathes.Any(path.StartsWith);
        }

        public void Dispose()
        {
            _watchers?.Dispose();
        }
    }
}