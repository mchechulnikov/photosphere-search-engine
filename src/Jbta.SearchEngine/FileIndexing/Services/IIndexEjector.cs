using Jbta.SearchEngine.Events;

namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal interface IIndexEjector
    {
        event FileIndexingEventHandler FileRemovedFromIndex;
        void Eject(string path);
    }
}