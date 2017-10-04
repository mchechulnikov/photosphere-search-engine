using System.Collections.Generic;
using System.IO;
using System.Threading;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class FileSupervisor : IFileSupervisor
    {
        private readonly IFileSystemWatcherFactory _watcherFactory;
        private readonly IDictionary<string, FileSystemWatcher> _watchers;
        private readonly ReaderWriterLockSlim _lock;

        public FileSupervisor(IFileSystemWatcherFactory watcherFactory)
        {
            _watcherFactory = watcherFactory;
            _watchers = new Dictionary<string, FileSystemWatcher>();
            _lock = new ReaderWriterLockSlim();
        }

        public IEnumerable<string> WatchedPathes => _watchers.Keys;

        public void Watch(string path)
        {
            var watcher = _watcherFactory.New(path);

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
    }
}