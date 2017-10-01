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
        private readonly IFileParser _fileParser;
        private readonly ITrie<WordEntry> _searchIndex;
        private readonly IDictionary<FileVersion, ISet<string>> _directIndex;
        private readonly IDictionary<string, ISet<FileVersion>> _fileVersions;

        public FileIndexer(
            IFileParser fileParser,
            ITrie<WordEntry> searchIndex)
        {
            _fileParser = fileParser;
            _searchIndex = searchIndex;
            _directIndex = new Dictionary<FileVersion, ISet<string>>();
            _fileVersions = new Dictionary<string, ISet<FileVersion>>();
        }

        public void Index(string path)
        {
            if (FileSystem.IsDirectory(path))
            {
                var filesPathes = FileSystem.GetFilesPathesByDirectory(path).ToArray();
                LoadFiles(filesPathes);
            }
            else
            {
                LoadFile(path);
            }
        }

        public void RemoveFromIndex(string path)
        {
            var fileVersions = _fileVersions[path];
            foreach (var fileVersion in fileVersions)
            {
                RemoveFileVersion(fileVersion);
            }
        }

        public void ChangeFilePath(string oldPath, string newPath)
        {
            var fileVersions = _fileVersions[oldPath];
            _fileVersions.Add(newPath, fileVersions);

            foreach (var fileVersion in fileVersions)
            {
                fileVersion.Path = newPath;
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

        private void LoadFiles(params string[] filePathes)
        {
            foreach (var filePath in filePathes)
            {
                LoadFile(filePath);
            }
        }

        private void LoadFile(string filePath)
        {
            var wordsEntries = _fileParser.Parse(filePath).ToArray();
            var setOfWords = new HashSet<string>();
            if (!wordsEntries.Any())
            {
                return;
            }

            foreach (var (word, wordEntry) in wordsEntries)
            {
                setOfWords.Add(word);
                _searchIndex.Add(word, wordEntry);
            }

            var fileVersion = AddFileVersion(filePath, wordsEntries);
            _directIndex.Add(fileVersion, setOfWords);
        }

        private FileVersion AddFileVersion(string filePath, IEnumerable<(string word, WordEntry)> wordsEntries)
        {
            if (!_fileVersions.TryGetValue(filePath, out var setOfFileVersions))
            {
                setOfFileVersions = new SortedSet<FileVersion>();
                _fileVersions.Add(filePath, setOfFileVersions);
            }
            var fileVersion = wordsEntries.First().Item2.FileVersion;
            setOfFileVersions.Add(fileVersion);
            return fileVersion;
        }

        private void RemoveFileVersion(FileVersion fileVersion)
        {
            var words = _directIndex[fileVersion];
            foreach (var word in words)
            {
                _searchIndex.Remove(word, we => we.FileVersion == fileVersion);
            }
            _directIndex.Remove(fileVersion);
            GC.Collect();
        }

        private void RemoveIrrelevantFileVersions(string filePath)
        {
            var fileVersions = _fileVersions[filePath];
            var lastFileVersion = fileVersions.Last();
            //var latestFileVersionDateTime = fileVersions.Max(fv => fv.Version);
            //var lastFileVersion = fileVersions.First(v => v.Version == latestFileVersionDateTime);
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
            var fileVersions = _fileVersions[filePath];
            return fileVersions.All(fv => fv.Version != File.GetLastWriteTimeUtc(filePath));
        }
    }
}