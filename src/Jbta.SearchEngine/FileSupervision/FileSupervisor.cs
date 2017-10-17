using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileSupervision.FileSystemEventWatching;
using Jbta.SearchEngine.FileSupervision.FileSystemPolling;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class FileSupervisor : IFileSupervisor
    {
        private readonly IEventReactor _eventReactor;
        private readonly FileSystemEventsProcessor _eventsProcessor;
        private readonly PathWatchersCollection _watchers;
        private readonly PathPoller _pathPoller;

        public FileSupervisor(
            IEventReactor eventReactor,
            FileSystemEventsProcessor eventsProcessor,
            PathWatchersCollection watchers,
            PathPoller pathPoller)
        {
            _eventReactor = eventReactor;
            _eventsProcessor = eventsProcessor;
            _watchers = watchers;
            _pathPoller = pathPoller;
        }

        public IEnumerable<string> WatchedPathes => _watchers.Pathes;

        public void Watch(string path)
        {
            var pathWatcher = new PathWatcher(path, _eventsProcessor.Add);
            _watchers.Add(path, pathWatcher);
            pathWatcher.Enable();
            _pathPoller.TryStart();

            _eventReactor.React(EngineEvent.PathWatchingStarted, path);
        }

        public void Unwatch(string path)
        {
            if (!_watchers.TryRemove(path))
            {
                return;
            }
            _pathPoller.TryStop();
            _eventReactor.React(EngineEvent.PathWatchingEnded, path);
        }

        public bool IsUnderWatching(string path)
        {
            return _watchers.Contains(path) || _watchers.Pathes.Any(path.StartsWith);
        }

        public void Dispose()
        {
            _watchers?.Dispose();
            _eventsProcessor?.Dispose();
            _pathPoller?.Dispose();
        }
    }
}