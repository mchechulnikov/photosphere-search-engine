namespace Jbta.SearchEngine.FileWatching
{
    internal interface IFileWatcher
    {
        void Watch(string path);
        void Unwatch(string path);
    }
}