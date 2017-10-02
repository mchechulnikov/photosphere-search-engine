using Jbta.SearchEngine.Events;

namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal interface IFileIndexer
    {
        event FileIndexingEventHandler FileIndexingStarted;
        event FileIndexingEventHandler FileIndexed;
        void Index(string path);
    }
}
