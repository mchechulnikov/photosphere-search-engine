using System;
using System.Threading;

namespace Jbta.SearchEngine.Utils
{
    internal static class ReaderWriterLockSlimExtensions
    {
        public static IDisposable ForReading(this ReaderWriterLockSlim l) => new ReadLock(l);

        public static IDisposable ForWriting(this ReaderWriterLockSlim l) => new WriteLock(l);

        private class ReadLock : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public ReadLock(ReaderWriterLockSlim l)
            {
                _lock = l;
                _lock.EnterReadLock();
            }

            public void Dispose()
            {
                _lock.ExitReadLock();
            }
        }

        private class WriteLock : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public WriteLock(ReaderWriterLockSlim l)
            {
                _lock = l;
                _lock.EnterWriteLock();
            }

            public void Dispose()
            {
                _lock.ExitWriteLock();
            }
        }
    }
}