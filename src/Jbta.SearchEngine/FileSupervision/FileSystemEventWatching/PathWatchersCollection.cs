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

        //public PathWatcher Get(string path)
        //{
        //    using (_lock.Shared())
        //    {
        //        return _watchers.TryGetValue(path, out var watcher) ? watcher : null;
        //    }
        //}

        public bool Contains(string path) => _watchers.ContainsKey(path);

        //public bool TryGet(string path, out PathWatcher watcher) =>
        //    _watchers.TryGetValue(path, out watcher);

        public void ReadPathes(Action<IEnumerable<string>> readAction)
        {
            using (_lock.Shared())
            {
                readAction(_watchers.Keys);
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

        public IEnumerable<string> GetAncestorsPathes(string path)
        {
            var normalizedPath = path.EndsWith("\\") ? path : path + "\\";
            using (_lock.Shared())
            {
                return _watchers
                    .Where(fv =>
                    {
                        var versionPath = fv.Key;
                        var isPathMatched =
                            versionPath.Equals(path)
                            || versionPath.Equals(normalizedPath)
                            || versionPath.StartsWith(normalizedPath);
                        return isPathMatched;
                    })
                    .Select(fv => fv.Key)
                    .ToList();
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