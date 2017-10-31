using Photosphere.SearchEngine.FileIndexing;

namespace Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers
{
    internal class CreateEventHandler
    {
        private readonly IFileIndexer _indexer;

        public CreateEventHandler(IFileIndexer indexer)
        {
            _indexer = indexer;
        }

        public void Handle(string path)
        {
            _indexer.Index(path);
        }
    }
}