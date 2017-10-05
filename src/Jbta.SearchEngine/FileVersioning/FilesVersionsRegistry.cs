using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileVersioning
{
    internal class FilesVersionsRegistry
    {
        private readonly IEventReactor _eventReactor;
        private readonly IDictionary<string, FileVersionsCollection> _fileVersions;
        private readonly ReaderWriterLockSlim _lock;

        public FilesVersionsRegistry(IEventReactor eventReactor)
        {
            _eventReactor = eventReactor;
            _fileVersions = new Dictionary<string, FileVersionsCollection>();
            _lock = new ReaderWriterLockSlim();
        }

        public bool Contains(string filePath)
        {
            using (_lock.Reading())
            {
                return _fileVersions.ContainsKey(filePath);
            }
        }

        public IEnumerable<string> Files
        {
            get
            {
                using (_lock.Reading())
                {
                    return _fileVersions.Keys;
                }
            }
        }

        public IReadOnlyCollection<FileVersion> RemoveDeadVersions()
        {
            var result = new List<FileVersion>();
            using (_lock.Reading())
            {
                foreach (var collection in _fileVersions.Values)
                {
                    var deadVersions = collection.RemoveDeadVersions();
                    result.AddRange(deadVersions);
                }
                return result;
            }
        }

        public FileVersion RegisterFileVersion(string filePath)
        {
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);
            var creationTimeUtc = File.GetCreationTimeUtc(filePath);
            var fileVersion = new FileVersion(filePath, lastWriteTimeUtc, creationTimeUtc);

            using (_lock.UpgradableRead())
            {
                if (!_fileVersions.TryGetValue(fileVersion.Path, out var fileVersionsCollection))
                {
                    fileVersionsCollection = new FileVersionsCollection();
                    using (_lock.Write())
                    {
                        _fileVersions.Add(fileVersion.Path, fileVersionsCollection);
                    }
                }
                fileVersionsCollection.Add(fileVersion);
            }
            return fileVersion;
        }

        public void RemoveAfterAction(string filePath, Action<IReadOnlyCollection<FileVersion>> action)
        {
            using (_lock.UpgradableRead())
            {
                var fileVersions = _fileVersions[filePath];
                action(fileVersions.Items.ToList());
                using (_lock.Write())
                {
                    _fileVersions.Remove(filePath);
                }
            }
        }

        public IReadOnlyCollection<FileVersion> Get(string filePath)
        {
            _fileVersions.TryGetValue(filePath, out var result);
            return result?.Items.ToList();
        }

        public void KillVersions(IEnumerable<FileVersion> irrelevantVersions)
        {
            foreach (var version in irrelevantVersions)
            {
                version.IsDead = true;
            }
        }

        public void KillVersions(string filePath)
        {
            using (_lock.Reading())
            {
                if (_fileVersions.TryGetValue(filePath, out var versions))
                {
                    versions.KillVersions();
                }
            }
        }

        public void DoActionIfFileUpdatable(string filePath, Action action)
        {
            FileVersionsCollection fileVersions;
            using (_lock.Reading())
            {
                if (!_fileVersions.TryGetValue(filePath, out fileVersions))
                {
                    //action();
                    return;
                }
            }
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);
            var creationTimeUtc = File.GetCreationTimeUtc(filePath);
            if (fileVersions.Items.All(fv =>
                fv.LastWriteDate != lastWriteTimeUtc || fv.CreationDate != creationTimeUtc))
            {
                action();
            }
            //fileVersions.IfAllThanDo(fv => fv.LastWriteDate != lastWriteTimeUtc && fv.CreationDate != creationTimeUtc, action);
            
        }

        public void ChangeFilePath(string oldPath, string newPath)
        {
            using (_lock.UpgradableRead())
            {
                if (!_fileVersions.TryGetValue(oldPath, out var fileVersions))
                {
                    return;
                }

                using (_lock.Write())
                {
                    _fileVersions.Add(newPath, fileVersions);
                    fileVersions.UpdateFilePath(newPath);
                    _fileVersions.Remove(oldPath);
                }
            }

            _eventReactor.React(EngineEvent.FilePathChanged, oldPath, newPath);
        }
    }
}