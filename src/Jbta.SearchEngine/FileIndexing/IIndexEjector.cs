namespace Jbta.SearchEngine.FileIndexing
{
    internal interface IIndexEjector
    {
        void Eject(string path);
        void EjectFile(string filePath);
    }
}