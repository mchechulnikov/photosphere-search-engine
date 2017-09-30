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
        private readonly IDictionary<string, ISet<string>> _directIndex;

        public FileIndexer(
            IFileParser fileParser,
            ITrie<WordEntry> searchIndex)
        {
            _fileParser = fileParser;
            _searchIndex = searchIndex;
            _directIndex = new Dictionary<string, ISet<string>>();
        }

        public void Index(string path)
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
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
            var words = _directIndex[path];
            foreach (var word in words)
            {
                _searchIndex.Remove(word, e => e.FileName == path);
            }
            _directIndex.Remove(path);
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
            foreach (var (word, wordEntry) in wordsEntries)
            {
                setOfWords.Add(word);
                _searchIndex.Add(word, wordEntry);
            }
            _directIndex.Add(filePath, setOfWords);
        }
    }
}