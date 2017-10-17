using System;

namespace Jbta.SearchEngine.Scheduling
{
    internal interface IScheduler : IDisposable
    {
        void Start();
    }
}