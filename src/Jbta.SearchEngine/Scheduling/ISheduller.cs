using System;

namespace Jbta.SearchEngine.Scheduling
{
    internal interface ISheduller : IDisposable
    {
        void Start();
    }
}