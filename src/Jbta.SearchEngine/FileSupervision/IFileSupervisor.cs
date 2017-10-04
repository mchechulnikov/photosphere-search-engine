using System;
using System.Collections.Generic;

namespace Jbta.SearchEngine.FileSupervision
{
    internal interface IFileSupervisor : IDisposable
    {
        IEnumerable<string> WatchedPathes { get; }
        bool IsUnderWatching(string path);
        void Watch(string path);
        void Unwatch(string path);
    }
}