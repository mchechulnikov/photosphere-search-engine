using Jbta.SearchEngine.FileIndexing;

namespace Jbta.SearchEngine.FileWatching
{
    internal class FileWatcher : IFileWatcher
    {
        private readonly IFileIndexer _fileIndexer;

        public FileWatcher(IFileIndexer fileIndexer)
        {
            _fileIndexer = fileIndexer;
        }

        public void Watch(string path)
        {
            // TODO subscribe from events
        }

        public void Unwatch(string path)
        {
            // TODO unsubscribe from events
            _fileIndexer.RemoveFromIndex(path);
        }
    }
}