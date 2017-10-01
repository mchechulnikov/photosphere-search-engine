using System;

namespace Jbta.SearchEngine.FileWatching
{
    internal interface IFileWatcher : IDisposable
    {
        void Watch(string path);
        void Unwatch(string path);
    }
}