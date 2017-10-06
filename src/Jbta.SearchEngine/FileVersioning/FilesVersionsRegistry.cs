using System.Collections.Generic;
using System.IO;
using System.Threading;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.Utils.Extensions;

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
            using (_lock.Shared())
            {
                return _fileVersions.ContainsKey(filePath);
            }
        }

        public IEnumerable<string> Files
        {
            get
            {
                using (_lock.Shared())
                {
                    return _fileVersions.Keys;
                }
            }
        }

        public IReadOnlyCollection<FileVersion> RemoveDeadVersions()
        {
            var result = new HashSet<FileVersion>();
            using (_lock.Shared())
            {
                foreach (var collection in _fileVersions.Values)
                {
                    var deadVersions = collection.RemoveDeadVersions();
                    foreach (var deadVersion in deadVersions)
                    {
                        result.Add(deadVersion);
                    }
                }
                return result;
            }
        }

        public FileVersion RegisterFileVersion(string filePath)
        {
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);
            var creationTimeUtc = File.GetCreationTimeUtc(filePath);
            var fileVersion = new FileVersion(filePath, lastWriteTimeUtc, creationTimeUtc);

            using (_lock.SharedIntentExclusive())
            {
                if (!_fileVersions.TryGetValue(fileVersion.Path, out var fileVersionsCollection))
                {
                    fileVersionsCollection = new FileVersionsCollection();
                    using (_lock.Exclusive())
                    {
                        _fileVersions.Add(fileVersion.Path, fileVersionsCollection);
                    }
                }
                fileVersionsCollection.Add(fileVersion);
            }
            return fileVersion;
        }

        public IReadOnlyCollection<FileVersion> Get(string filePath)
        {
            _fileVersions.TryGetValue(filePath, out var versionsCollection);
            return versionsCollection?.GetList();
        }

        public void KillVersions(IEnumerable<FileVersion> irrelevantVersions)
        {
            foreach (var version in irrelevantVersions)
            {
                version.IsDead = true;
            }
        }

        public void KillAllVersions(string filePath)
        {
            using (_lock.Shared())
            {
                if (_fileVersions.TryGetValue(filePath, out var versions))
                {
                    versions.KillVersions();
                }
            }
        }

        public bool IsFileUpdatable(string filePath)
        {
            FileVersionsCollection fileVersions;
            using (_lock.Shared())
            {
                if (!_fileVersions.TryGetValue(filePath, out fileVersions))
                {
                    return false;
                }
            }
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);
            var creationTimeUtc = File.GetCreationTimeUtc(filePath);
            return fileVersions.All(fv => fv.LastWriteDate != lastWriteTimeUtc || fv.CreationDate != creationTimeUtc);
        }

        public void ChangeFilePath(string oldPath, string newPath)
        {
            using (_lock.SharedIntentExclusive())
            {
                if (!_fileVersions.TryGetValue(oldPath, out var fileVersions))
                {
                    return;
                }

                using (_lock.Exclusive())
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