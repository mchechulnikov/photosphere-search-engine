using System;
using System.Collections.Generic;
using System.IO;
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
                var kvs = _watchers.Where(w => w.Key.StartsWith(oldPath)).ToList();
                if (!kvs.Any())
                {
                    return;
                }
                foreach (var kv in kvs)
                {
                    var (oldWatcherPath, watcher) = (kv.Key, kv.Value);
                    var newWatcherPath = newPath + oldWatcherPath.Replace(oldPath, string.Empty);

                    watcher.Reset(newWatcherPath);
                    _watchers.Add(newWatcherPath, watcher);
                    _watchers.Remove(oldWatcherPath);
                }
            }
        }

        public bool TryRemove(string path)
        {
            using (_lock.SharedIntentExclusive())
            {
                var kvs = _watchers.Where(w => w.Key.StartsWith(path)).ToList();
                if (!kvs.Any())
                {
                    return false;
                }
                foreach (var kv in kvs)
                {
                    var (watcherPath, watcher) = (kv.Key, kv.Value);
                    using (_lock.Exclusive())
                    {
                        watcher.Dispose();
                        _watchers.Remove(watcherPath);
                    }
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