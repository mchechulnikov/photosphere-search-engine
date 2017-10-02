using System.Collections.Generic;
using System.IO;
using System.Threading;
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
        private readonly IDictionary<string, FileSystemWatcher> _watchers;
        private readonly ReaderWriterLockSlim _lock;

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
            _watchers = new Dictionary<string, FileSystemWatcher>();
            _lock = new ReaderWriterLockSlim();
        }

        public IEnumerable<string> WatchedPathes => _watchers.Keys;

        public void Watch(string path)
        {
            var watcher = GetNewWatcher(path);

            using (_lock.ForWriting())
            {
                _watchers.Add(path, watcher);
            }

            watcher.EnableRaisingEvents = true;
        }

        public void Unwatch(string path)
        {
            using (_lock.ForWriting())
            {
                if (!_watchers.TryGetValue(path, out var watcher))
                {
                    return;
                }
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                _watchers.Remove(path);
            }
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