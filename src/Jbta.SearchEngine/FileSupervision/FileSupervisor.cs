using System.Collections.Generic;
using System.Linq;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class FileSupervisor : IFileSupervisor
    {
        private readonly PathWatcherFactory _pathWatcherFactory;
        private readonly WatchersCollection _watchers;

        public FileSupervisor(
            PathWatcherFactory pathWatcherFactory,
            WatchersCollection watchers)
        {
            _pathWatcherFactory = pathWatcherFactory;
            _watchers = watchers;
        }

        public IEnumerable<string> WatchedPathes => _watchers.Pathes;

        public void Watch(string path)
        {
            var watchersSquad = _pathWatcherFactory.New(path);
            _watchers.Add(path, watchersSquad);
            watchersSquad.Enable();
        }

        public void Unwatch(string path)
        {
            var watchersSquad = _watchers.Get(path);
            if (watchersSquad == null)
            {
                return;
            }

            watchersSquad.Dispose();
            _watchers.Remove(path);
        }

        public bool IsUnderWatching(string path)
        {
            return _watchers.Contains(path) || _watchers.Pathes.Any(path.StartsWith);
        }

        public void Dispose()
        {
            _watchers?.Dispose();
        }
    }
}