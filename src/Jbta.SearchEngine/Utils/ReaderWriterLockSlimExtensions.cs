using System;
using System.Threading;

namespace Jbta.SearchEngine.Utils
{
    internal static class ReaderWriterLockSlimExtensions
    {
        public static IDisposable Reading(this ReaderWriterLockSlim l) => new ReadLock(l);

        public static IDisposable UpgradableRead(this ReaderWriterLockSlim l) => new UpgradableReadLock(l);

        public static IDisposable Write(this ReaderWriterLockSlim l) => new WriteLock(l);

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

        private class UpgradableReadLock : IDisposable
        {
            private readonly ReaderWriterLockSlim _lock;

            public UpgradableReadLock(ReaderWriterLockSlim l)
            {
                _lock = l;
                _lock.EnterUpgradeableReadLock();
            }

            public void Dispose()
            {
                _lock.ExitUpgradeableReadLock();
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