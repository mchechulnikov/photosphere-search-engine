using System;
using System.Collections.Generic;

namespace Photosphere.SearchEngine.FileSupervision
{
    internal interface IFileSupervisor : IDisposable
    {
        IEnumerable<string> WatchedPathes { get; }
        void Watch(string path);
        void Unwatch(string path);
        bool IsUnderWatching(string path);
    }
}