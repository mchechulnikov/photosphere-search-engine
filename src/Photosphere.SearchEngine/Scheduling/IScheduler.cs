using System;

namespace Photosphere.SearchEngine.Scheduling
{
    internal interface IScheduler : IDisposable
    {
        void Start();
    }
}