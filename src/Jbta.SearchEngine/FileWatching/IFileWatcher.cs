using System;
using System.Collections.Generic;

namespace Jbta.SearchEngine.FileWatching
{
    internal interface IFileWatcher : IDisposable
    {
        IEnumerable<string> WatchedPathes { get; }
        void Watch(string path);
        void Unwatch(string path);
    }
}