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
        private readonly IDictionary<string, ISet<FileVersion>> _fileVersions;
        private readonly ReaderWriterLockSlim _lock;

        public FilesVersionsRegistry(IEventReactor eventReactor)
        {
            _eventReactor = eventReactor;
            _fileVersions = new Dictionary<string, ISet<FileVersion>>();
            _lock = new ReaderWriterLockSlim();
        }

        public bool Contains(string filePath)
        {
            using (_lock.ForReading())
            {
                return _fileVersions.ContainsKey(filePath);
            }
        }

        public IEnumerable<string> Files
        {
            get
            {
                using (_lock.ForReading())
                {
                    return _fileVersions.Keys;
                }
            }
        }

        public FileVersion RegisterFileVersion(string filePath)
        {
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);
            var fileVersion = new FileVersion(filePath, lastWriteTimeUtc);

            using (_lock.ForWriting())
            {
                if (!_fileVersions.TryGetValue(fileVersion.Path, out var setOfFileVersions))
                {
                    setOfFileVersions = new SortedSet<FileVersion>();
                    _fileVersions.Add(fileVersion.Path, setOfFileVersions);
                }
                setOfFileVersions.Add(fileVersion);
            }
            return fileVersion;
        }

        public IEnumerable<FileVersion> Get(string filePath)
        {
            using (_lock.ForReading())
            {
                return _fileVersions[filePath];
            }
        }

        public void Remove(string filePath)
        {
            using (_lock.ForWriting())
            {
                _fileVersions.Remove(filePath);
            }
        }

        public IReadOnlyCollection<FileVersion> RemoveIrrelevantFileVersions(string filePath)
        {
            using (_lock.ForWriting())
            {
                var fileVersions = _fileVersions[filePath];
                var lastFileVersion = fileVersions.Last();
                var irrelevantFileVersions = fileVersions.Where(fv => fv != lastFileVersion).ToList();
                foreach (var fileVersion in irrelevantFileVersions)
                {
                    fileVersions.Remove(fileVersion);
                }
                return irrelevantFileVersions;
            }
        }

        public bool IsNeedToBeUpdated(string filePath)
        {
            using (_lock.ForWriting())
            {
                return _fileVersions.TryGetValue(filePath, out var fileVersions)
                       && fileVersions.All(fv => fv.Version != File.GetLastWriteTimeUtc(filePath));
            }
        }

        public void ChangeFilePath(string oldPath, string newPath)
        {
            using (_lock.ForWriting())
            {
                if (!_fileVersions.TryGetValue(oldPath, out var fileVersions))
                {
                    return;
                }

                _fileVersions.Add(newPath, fileVersions);

                foreach (var fileVersion in fileVersions)
                {
                    fileVersion.Path = newPath;
                }

                _fileVersions.Remove(oldPath);
            }

            _eventReactor.React(EngineEvent.FilePathChanged, oldPath, newPath);
        }
    }
}