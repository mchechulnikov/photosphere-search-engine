using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Jbta.SearchEngine.Utils.Extensions;

namespace Jbta.SearchEngine.FileVersioning
{
    internal class FilesVersionsRegistry
    {
        private readonly IDictionary<string, FileVersionsCollection> _fileVersions;
        private readonly ReaderWriterLockSlim _lock;

        public FilesVersionsRegistry()
        {
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

        public IEnumerable<string> GetAliveFiles(string path)
        {
            var normalizedPath = path.EndsWith("\\") ? path : path + "\\";
            using (_lock.Shared())
            {
                return _fileVersions
                    .Where(fv =>
                    {
                        var versionPath = fv.Key;
                        var isPathMatched =
                            versionPath.Equals(path)
                            || versionPath.Equals(normalizedPath)
                            || versionPath.StartsWith(normalizedPath);
                        return isPathMatched && fv.Value.AnyAlive();
                    })
                    .Select(fv => fv.Key)
                    .ToList();
            }
        }

        public IReadOnlyCollection<FileVersion> RemoveDeadVersions()
        {
            var result = new HashSet<FileVersion>();
            using (_lock.SharedIntentExclusive())
            {
                foreach (var kv in _fileVersions)
                {
                    var collection = kv.Value;
                    var deadVersions = collection.RemoveDeadVersions();
                    foreach (var deadVersion in deadVersions)
                    {
                        result.Add(deadVersion);
                    }

                    // TODO this is thread unsafe
                    if (collection.Any())
                    {
                        continue;
                    }

                    using (_lock.Exclusive())
                    {
                        _fileVersions.Remove(kv.Key);
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
        }
    }
}