using Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching;
using Photosphere.SearchEngine.Utils;

namespace Photosphere.SearchEngine.FileSupervision.FileSystemPolling
{
    internal class DeadPathDetector
    {
        private readonly PathWatchersCollection _watchers;
        private readonly PathRemover _pathRemover;

        public DeadPathDetector(
            PathWatchersCollection watchers,
            PathRemover pathRemover)
        {
            _watchers = watchers;
            _pathRemover = pathRemover;
        }

        public void Clean()
        {
            foreach (var path in _watchers.Pathes)
            {
                if (FileSystem.IsRemovedButLocked(path))
                {
                    _pathRemover.Remove(path);
                }
            }
        }
    }
}