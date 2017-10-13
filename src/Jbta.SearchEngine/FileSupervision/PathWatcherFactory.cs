using System.IO;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileVersioning.Services;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class PathWatcherFactory
    {
        private readonly IEventReactor _eventReactor;
        private readonly IFileIndexer _indexer;
        private readonly IIndexUpdater _indexUpdater;
        private readonly IIndexEjector _indexEjector;
        private readonly IFilePathActualizer _filePathActualizer;
        private readonly WatchersCollection _watchers;

        public PathWatcherFactory(
            IEventReactor eventReactor,
            IFileIndexer indexer,
            IIndexUpdater indexUpdater,
            IIndexEjector indexEjector,
            IFilePathActualizer filePathActualizer,
            WatchersCollection watchers)
        {
            _eventReactor = eventReactor;
            _indexer = indexer;
            _indexUpdater = indexUpdater;
            _indexEjector = indexEjector;
            _filePathActualizer = filePathActualizer;
            _watchers = watchers;
        }

        public PathWatcher New(string path) => 
            new PathWatcher(path, SubscribeOnFileEvents, SubscribeOnParentDirectoryEvents);

        private void SubscribeOnFileEvents(FileSystemWatcher watcher)
        {
            watcher.Created += (o, e) =>
            {
                _indexer.Index(e.FullPath);
            };
            watcher.Changed += (o, e) =>
            {
                // Event occurs even on creations or deletions of files.
                // Sometimes it can be raised twice or more times sequently.
                // https://stackoverflow.com/a/1765094/4569169

                if (e.ChangeType != WatcherChangeTypes.Changed)
                {
                    return;
                }
                if (!FileSystem.IsExistingPath(e.FullPath))
                {
                    return;
                }
                if (FileSystem.IsDirectory(e.FullPath))
                {
                    return;
                }

                _indexUpdater.Update(e.FullPath);
            };
            watcher.Deleted += (o, e) =>
            {
                if (e.ChangeType != WatcherChangeTypes.Deleted)
                {
                    return;
                }

                if (_watchers.Contains(e.FullPath))
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    _watchers.Remove(e.FullPath);
                    _eventReactor.React(EngineEvent.PathWatchingEnded, e.FullPath);
                }
                _indexEjector.EjectFile(e.FullPath);
            };
            watcher.Renamed += (o, e) =>
            {
                _filePathActualizer.Actualize(e.OldFullPath, e.FullPath);
            };
        }

        private void SubscribeOnParentDirectoryEvents(FileSystemWatcher watcher)
        {
            watcher.Changed += (o, e) =>
            {
                // Event occurs even on creations or deletions of files.
                // Sometimes it can be raised twice or more times sequently.
                // https://stackoverflow.com/a/1765094/4569169

                if (e.ChangeType != WatcherChangeTypes.Changed)
                {
                    return;
                }
                if (!FileSystem.IsExistingPath(e.FullPath))
                {
                    return;
                }

                if (_watchers.Contains(e.FullPath))
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    _watchers.Remove(e.FullPath);
                    _eventReactor.React(EngineEvent.PathWatchingEnded, e.FullPath);
                }
            };
            watcher.Deleted += (o, e) =>
            {
                if (e.ChangeType != WatcherChangeTypes.Deleted)
                {
                    return;
                }

                if (_watchers.Contains(e.FullPath))
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Dispose();
                    _watchers.Remove(e.FullPath);
                    _eventReactor.React(EngineEvent.PathWatchingEnded, e.FullPath);
                }
            };
            watcher.Renamed += (o, e) =>
            {
                _filePathActualizer.Actualize(e.OldFullPath, e.FullPath);
            };
        }
    }
}