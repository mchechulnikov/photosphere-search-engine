using System;

namespace Jbta.SearchEngine.Shedulling
{
    internal interface ISheduller : IDisposable
    {
        void Start();
    }
}