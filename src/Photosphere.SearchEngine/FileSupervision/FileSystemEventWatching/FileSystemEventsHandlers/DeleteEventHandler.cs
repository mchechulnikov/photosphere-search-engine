using Photosphere.SearchEngine.Events;
using Photosphere.SearchEngine.FileIndexing;

namespace Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers
{
    internal class DeleteEventHandler
    {
        private readonly IEventReactor _eventReactor;
        private readonly IIndexEjector _indexEjector;
        private readonly PathWatchersCollection _watchers;

        public DeleteEventHandler(
            IEventReactor eventReactor,
            IIndexEjector indexEjector,
            PathWatchersCollection watchers)
        {
            _eventReactor = eventReactor;
            _watchers = watchers;
            _indexEjector = indexEjector;
        }

        public void Handle(string path)
        {
            if (_watchers.TryRemove(path))
            {
                _eventReactor.React(EngineEvent.PathWatchingEnded, path);
            }

            _indexEjector.Eject(path);
        }
    }
}