using System.IO;

namespace Jbta.SearchEngine.FileSupervision
{
    internal interface IFileSystemWatcherFactory
    {
        FileSystemWatcher New(string path);
    }
}