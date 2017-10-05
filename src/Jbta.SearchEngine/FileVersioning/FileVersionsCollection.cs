using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileVersioning
{
    internal class FileVersionsCollection
    {
        private readonly ReaderWriterLockSlim _lock;

        public FileVersionsCollection()
        {
            _lock = new ReaderWriterLockSlim();
        }

        public ISet<FileVersion> Items { get; } = new SortedSet<FileVersion>();

        public void Add(FileVersion fileVersion)
        {
            using (_lock.Write())
            {
                Items.Add(fileVersion);
            }
        }

        public void UpdateFilePath(string newPath)
        {
            using (_lock.UpgradableRead())
            {
                foreach (var fileVersion in Items)
                {
                    using (_lock.Write())
                    {
                        fileVersion.Path = newPath;
                    }
                }
            }
        }

        //public void RemoveAllExceptAfterAction(
        //    DateTime exceptVersion,
        //    Action<IReadOnlyCollection<FileVersion>> action)
        //{
        //    //using (_lock.UpgradableRead())
        //    //{
        //    var lastFileVersion = Items.Where(x => x.LastWriteDate == exceptVersion);
        //    var irrelevantFileVersions = Items.Where(fv => fv != lastFileVersion).ToList();

        //    action(irrelevantFileVersions);

        //    foreach (var fileVersion in irrelevantFileVersions)
        //    {
        //        using (_lock.Write())
        //        {
        //            Items.Remove(fileVersion);
        //        }
        //    }
        //    //}
        //}

        public void IfAllThanDo(Func<FileVersion, bool> predicte, Action action)
        {
            using (_lock.UpgradableRead())
            {
                if (Items.All(predicte))
                {
                    action();
                }
            }
        }

        public IEnumerable<FileVersion> RemoveDeadVersions()
        {
            using (_lock.UpgradableRead())
            {
                var deadVersions = Items.Where(i => i.IsDead).ToList();
                foreach (var deadVersion in deadVersions)
                {
                    using (_lock.Write())
                    {
                        Items.Remove(deadVersion);
                    }
                }
                return deadVersions;
            }
        }

        public void KillVersions()
        {
            using (_lock.Write())
            {
                foreach (var version in Items)
                {
                    version.IsDead = true;
                }
            }
        }

        public IReadOnlyCollection<FileVersion> ToList()
        {
            using (_lock.Reading())
            {
                return Items.ToList();
            }
        }
    }
}