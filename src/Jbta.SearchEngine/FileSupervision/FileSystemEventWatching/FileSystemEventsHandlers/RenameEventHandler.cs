using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileVersioning.Services;

namespace Jbta.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers
{
    internal class RenameEventHandler
    {
        private readonly IEventReactor _eventReactor;
        private readonly IFilePathActualizer _filePathActualizer;
        private readonly PathWatchersCollection _watchers;

        public RenameEventHandler(
            IEventReactor eventReactor,
            IFilePathActualizer filePathActualizer,
            PathWatchersCollection watchers)
        {
            _eventReactor = eventReactor;
            _filePathActualizer = filePathActualizer;
            _watchers = watchers;
        }

        public void Handle(string oldPath, string newPath)
        {
            _filePathActualizer.Actualize(oldPath, newPath);
            _watchers.ChangePath(oldPath, newPath);
            _eventReactor.React(EngineEvent.PathChanged, oldPath, newPath);
        }
    }
}