namespace Jbta.SearchEngine.FileIndexing
{
    internal interface IFileIndexer
    {
        void Index(string path);
        void RemoveFromIndex(string path);
        void UpdateIndex(string filePath);
        void ChangePath(string oldPath, string newPath);
    }
}
