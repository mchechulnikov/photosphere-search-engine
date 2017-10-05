using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jbta.SearchEngine.Vendor.NonBlocking.ConcurrentDictionary;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class FileSupervisor : IFileSupervisor
    {
        private readonly FileSystemWatcherFactory _watcherFactory;
        private readonly ConcurrentDictionary<string, FileSystemWatcher> _watchers;

        public FileSupervisor(FileSystemWatcherFactory watcherFactory)
        {
            _watcherFactory = watcherFactory;
            _watchers = new ConcurrentDictionary<string, FileSystemWatcher>();
        }

        public IEnumerable<string> WatchedPathes => _watchers.Keys;

        public void Watch(string path)
        {
            var watcher = _watcherFactory.New(path);
            _watchers.AddOrUpdate(path, watcher, (k, v) => v);
            watcher.EnableRaisingEvents = true;
        }

        public void Unwatch(string path)
        {
            if (!_watchers.TryGetValue(path, out var watcher))
            {
                return;
            }

            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
            _watchers.Remove(path);
        }

        public bool IsUnderWatching(string path)
        {
            return _watchers.ContainsKey(path) || _watchers.Keys.Any(path.StartsWith);
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