using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class FilesVersionsRegistry
    {
        private readonly IDictionary<string, ISet<FileVersion>> _fileVersions;

        public FilesVersionsRegistry()
        {
            _fileVersions = new Dictionary<string, ISet<FileVersion>>();
        }

        public bool Contains(string filePath) => _fileVersions.ContainsKey(filePath);

        public IEnumerable<string> Files => _fileVersions.Keys;

        public IEnumerable<FileVersion> Get(string filePath) => _fileVersions[filePath];

        public void Remove(string filePath) => _fileVersions.Remove(filePath);

        public IReadOnlyCollection<FileVersion> RemoveIrrelevantFileVersions(string filePath)
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

        public FileVersion RegisterFileVersion(string filePath)
        {
            var lastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);
            var fileVersion = new FileVersion(filePath, lastWriteTimeUtc);

            if (!_fileVersions.TryGetValue(fileVersion.Path, out var setOfFileVersions))
            {
                setOfFileVersions = new SortedSet<FileVersion>();
                _fileVersions.Add(fileVersion.Path, setOfFileVersions);
            }
            setOfFileVersions.Add(fileVersion);
            return fileVersion;
        }

        public bool IsNeedToBeUpdated(string filePath)
        {
            return _fileVersions.TryGetValue(filePath, out var fileVersions)
                && fileVersions.All(fv => fv.Version != File.GetLastWriteTimeUtc(filePath));
        }

        public void ChangePath(string oldPath, string newPath)
        {
            if (FileSystem.IsDirectory(newPath))
            {
                var oldFilesPathes = Files.Where(p => p.StartsWith(oldPath)).ToList();
                foreach (var oldFilePath in oldFilesPathes)
                {
                    var newFilePath = newPath + oldFilePath.Substring(oldPath.Length);
                    ChangeFilePath(oldFilePath, newFilePath);
                }
            }
            else
            {
                ChangeFilePath(oldPath, newPath);
            }
        }

        private void ChangeFilePath(string oldPath, string newPath)
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
    }
}