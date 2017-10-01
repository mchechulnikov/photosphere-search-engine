using System.Collections.Concurrent;
using System.IO;
using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileWatching
{
    internal class FileWatcher : IFileWatcher
    {
        private const NotifyFilters WatcherNotifyFilters =
            NotifyFilters.LastWrite
            | NotifyFilters.FileName
            | NotifyFilters.DirectoryName;

        private readonly IFileIndexer _fileIndexer;
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers;

        public FileWatcher(IFileIndexer fileIndexer)
        {
            _fileIndexer = fileIndexer;
            _watchers = new ConcurrentDictionary<string, FileSystemWatcher>();
        }

        public void Watch(string path)
        {
            var watcher = GetNewWatcher(path);
            _watchers.AddOrUpdate(path, watcher, (k, v) => v);
            watcher.EnableRaisingEvents = true;
        }

        public void Unwatch(string path)
        {
            if (_watchers.TryRemove(path, out var watcher))
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            _fileIndexer.RemoveFromIndex(path);
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
            watcher.Created += (o, e) => _fileIndexer.Index(e.FullPath);
            watcher.Changed += (o, e) => _fileIndexer.UpdateIndex(e.FullPath);;
            watcher.Deleted += (o, e) => _fileIndexer.RemoveFromIndex(e.FullPath);
            watcher.Renamed += (o, e) => _fileIndexer.ChangePath(e.OldFullPath, e.FullPath);
            return watcher;
        }
    }
}