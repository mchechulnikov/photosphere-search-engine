using Jbta.SearchEngine.FileIndexing;

namespace Jbta.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers
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