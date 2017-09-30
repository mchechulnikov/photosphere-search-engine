namespace Jbta.SearchEngine.FileIndexing
{
    internal interface IFileIndexer
    {
        void Index(string path);
        void RemoveFromIndex(string path);
    }
}
