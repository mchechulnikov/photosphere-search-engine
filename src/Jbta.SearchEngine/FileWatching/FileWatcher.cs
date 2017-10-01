using System.Collections.Concurrent;
using System.IO;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.FileIndexing.Services;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileWatching
{
    internal class FileWatcher : IFileWatcher
    {
        private const NotifyFilters WatcherNotifyFilters =
            NotifyFilters.LastWrite
            | NotifyFilters.FileName
            | NotifyFilters.DirectoryName;

        private readonly IFileIndexer _indexer;
        private readonly IIndexUpdater _indexUpdater;
        private readonly IIndexEjector _indexEjector;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers;

        public FileWatcher(
            IFileIndexer indexer,
            IIndexUpdater indexUpdater,
            IIndexEjector indexEjector,
            FilesVersionsRegistry filesVersionsRegistry)
        {
            _indexer = indexer;
            _indexUpdater = indexUpdater;
            _indexEjector = indexEjector;
            _filesVersionsRegistry = filesVersionsRegistry;
            _watchers = new ConcurrentDictionary<string, FileSystemWatcher>();
        }

        public void Watch(string path)
        {
            _indexer.Index(path);

            var watcher = GetNewWatcher(path);
            _watchers.AddOrUpdate(path, watcher, (k, v) => v);

            watcher.EnableRaisingEvents = true;
        }

        public void Unwatch(string path)
        {
            if (!_watchers.TryRemove(path, out var watcher))
            {
                return;
            }
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }

        public void Dispose()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
        }

        private FileSystemWatcher GetNewWatcher(string path)
        {
            // TODO check path for already added
            var watcher = new FileSystemWatcher { NotifyFilter = WatcherNotifyFilters };
            if (FileSystem.IsDirectory(path))
            {
                watcher.Path = path;
                watcher.IncludeSubdirectories = true;
            }
            else
            {
                watcher.Path = FileSystem.GetDirectoryPathByFilePath(path);
                watcher.Filter = new FileInfo(path).Name;
            }
            watcher.Created += (o, e) => _indexer.Index(e.FullPath);
            watcher.Changed += (o, e) => _indexUpdater.Update(e.FullPath);;
            watcher.Deleted += (o, e) => _indexEjector.Eject(e.FullPath);
            watcher.Renamed += (o, e) => _filesVersionsRegistry.ChangePath(e.OldFullPath, e.FullPath);
            return watcher;
        }
    }
}