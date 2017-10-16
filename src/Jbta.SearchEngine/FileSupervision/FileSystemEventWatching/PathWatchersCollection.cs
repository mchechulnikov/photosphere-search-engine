using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.Utils.Extensions;

namespace Jbta.SearchEngine.FileSupervision.FileSystemEventWatching
{
    internal class PathWatchersCollection : IDisposable
    {
        private readonly IDictionary<string, PathWatcher> _watchers;
        private readonly ReaderWriterLockSlim _lock;

        public PathWatchersCollection()
        {
            _watchers = new Dictionary<string, PathWatcher>();
            _lock = new ReaderWriterLockSlim();
        }

        public IEnumerable<string> Pathes
        {
            get
            {
                using (_lock.Shared())
                {
                    return _watchers.Keys.ToList();
                }
            }
        }

        public void Add(string path, PathWatcher pathWatcher)
        {
            using (_lock.Exclusive())
            {
                _watchers.Add(path, pathWatcher);
            }
        }

        public bool Contains(string path) => _watchers.ContainsKey(path);

        public void ReadPathes(Action<IEnumerable<string>> readAction)
        {
            using (_lock.Shared())
            {
                readAction(_watchers.Keys);
            }
        }

        public void ChangePath(string oldPath, string newPath)
        {
            using (_lock.Exclusive())
            {
                if (!_watchers.TryGetValue(oldPath, out var watcher))
                {
                    return;
                }
                watcher.Reset(newPath);
                _watchers.Add(newPath, watcher);
                _watchers.Remove(oldPath);
            }
        }

        public bool TryRemove(string path)
        {
            using (_lock.SharedIntentExclusive())
            {
                if (!_watchers.TryGetValue(path, out var watcher))
                {
                    return false;
                }

                using (_lock.Exclusive())
                {
                    watcher.Dispose();
                    _watchers.Remove(path);
                }
            }
            return true;
        }

        public void Dispose()
        {
            using (_lock.Exclusive())
            {
                foreach (var watcher in _watchers.Values)
                {
                    watcher.Dispose();
                }
            }
        }
    }
}