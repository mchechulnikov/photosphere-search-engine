using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.Index;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class FileIndexer : IFileIndexer
    {
        private readonly FileParserProvider _parserProvider;
        private readonly ITrie<WordEntry> _searchIndex;
        private readonly IDictionary<FileVersion, ISet<string>> _directIndex;
        private readonly IDictionary<string, ISet<FileVersion>> _fileVersions;
        private readonly Settings _settings;

        public FileIndexer(
            FileParserProvider parserProvider,
            ITrie<WordEntry> searchIndex,
            Settings settings)
        {
            _parserProvider = parserProvider;
            _searchIndex = searchIndex;
            _settings = settings;
            _directIndex = new Dictionary<FileVersion, ISet<string>>();
            _fileVersions = new Dictionary<string, ISet<FileVersion>>();
        }

        public void Index(string path)
        {
            if (FileSystem.IsDirectory(path))
            {
                var filesPathes = FileSystem.GetFilesPathesByDirectory(path).ToArray();
                foreach (var filePath in filesPathes)
                {
                    LoadFile(filePath);
                }
            }
            else
            {
                LoadFile(path);
            }
        }

        public void RemoveFromIndex(string path)
        {
            if (_fileVersions.ContainsKey(path))
            {
                RemoveFilesFromIndex(path);
            }
            else
            {
                var filesPathes = _fileVersions.Keys.Where(p => p.StartsWith(path));
                foreach (var filesPath in filesPathes)
                {
                    RemoveFilesFromIndex(filesPath);
                }
            }
        }

        public void ChangePath(string oldPath, string newPath)
        {
            if (FileSystem.IsDirectory(newPath))
            {
                var oldFilesPathes = _fileVersions.Keys.Where(p => p.StartsWith(oldPath)).ToList();
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

        public void UpdateIndex(string filePath)
        {
            if (!IsUniqueChange(filePath))
            {
                return;
            }

            Index(filePath);
            RemoveIrrelevantFileVersions(filePath);
        }

        private void LoadFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (_settings.SupportedFilesExtensions.Contains(extension))
            {
                return;
            }

            var fileVersion = RegisterFileVersion(filePath);

            var fileParser = _parserProvider.Provide(filePath);
            var wordsEntries = fileParser.Parse(fileVersion).ToArray();
            var setOfWords = new HashSet<string>();
            _directIndex.Add(fileVersion, setOfWords);

            if (!wordsEntries.Any())
            {
                return;
            }

            foreach (var (word, wordEntry) in wordsEntries)
            {
                setOfWords.Add(word);
                _searchIndex.Add(word, wordEntry);
            }
        }

        private FileVersion RegisterFileVersion(string filePath)
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

        private void RemoveFilesFromIndex(string filePath)
        {
            var fileVersions = _fileVersions[filePath];
            foreach (var fileVersion in fileVersions)
            {
                RemoveFileVersion(fileVersion);
            }
        }

        private void RemoveFileVersion(FileVersion fileVersion)
        {
            var words = _directIndex[fileVersion];
            foreach (var word in words)
            {
                _searchIndex.Remove(word, we => we.FileVersion == fileVersion);
            }
            _directIndex.Remove(fileVersion);

            if (_settings.GcCollect)
            {
                GC.Collect();
            }
        }

        private void RemoveIrrelevantFileVersions(string filePath)
        {
            var fileVersions = _fileVersions[filePath];
            var lastFileVersion = fileVersions.Last();
            var irrelevantFileVersions = fileVersions.Where(fv => fv != lastFileVersion).ToList();
            if (!irrelevantFileVersions.Any())
            {
                return;
            }

            var words = irrelevantFileVersions.SelectMany(fv => _directIndex[fv]);
            foreach (var word in words)
            {
                _searchIndex.Remove(word, e => irrelevantFileVersions.Contains(e.FileVersion));
            }

            foreach (var fileVersion in irrelevantFileVersions)
            {
                _directIndex.Remove(fileVersion);
                fileVersions.Remove(fileVersion);
            }
        }

        private bool IsUniqueChange(string filePath)
        {
            if (_fileVersions.TryGetValue(filePath, out var fileVersions))
            {
                return fileVersions.All(fv => fv.Version != File.GetLastWriteTimeUtc(filePath));
            }
            return false;
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