using System;
using System.Collections.Generic;
using System.Threading;
using Jbta.SearchEngine.Utils.Extensions;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class WatchersCollection : IDisposable
    {
        private readonly IDictionary<string, PathWatcher> _watchers;
        private readonly ReaderWriterLockSlim _lock;

        public WatchersCollection()
        {
            _watchers = new Dictionary<string, PathWatcher>();
            _lock = new ReaderWriterLockSlim();
        }

        public IEnumerable<string> Pathes => _watchers.Keys;

        public void Add(string path, PathWatcher pathWatcher)
        {
            using (_lock.Exclusive())
            {
                _watchers.Add(path, pathWatcher);
            }
        }

        public PathWatcher Get(string path)
        {
            using (_lock.Shared())
            {
                return _watchers.TryGetValue(path, out var watcher) ? watcher : null;
            }
        }

        public bool Contains(string path) => _watchers.ContainsKey(path);

        public void Remove(string path)
        {
            using (_lock.Exclusive())
            {
                _watchers.Remove(path);
            }
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