using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Jbta.Indexing.Indexing;
using Jbta.Indexing.Utils;

namespace Jbta.Indexing
{
    public class Indexer : IIndexer
    {
        private readonly IDictionary<string, ISet<string>> _directIndex;
        private readonly ITrie<WordEntry> _invertedIndex;

        public Indexer()
        {
            _invertedIndex = new PatriciaTrie<WordEntry>();
            _directIndex = new Dictionary<string, ISet<string>>();
        }

        public void Add(string path)
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

        public void Remove(string path)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WordEntry> Search(string query, bool isCaseSensetive, bool isWholeWord)
        {
            return _invertedIndex.Get(query);
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
            var wordsEntries = GetWords(filePath).ToArray();
            var setOfWords = new HashSet<string>();
            foreach (var (word, wordEntry) in wordsEntries)
            {
                setOfWords.Add(word);
                _invertedIndex.Add(word, wordEntry);
            }
            _directIndex.Add(filePath, setOfWords);
        }

        private static IEnumerable<(string, WordEntry)> GetWords(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                const int bufferSize = 2048;
                var buffer = new char[bufferSize];
                var word = new StringBuilder();
                var position = 2;
                var lineNumber = 1;
                while (reader.ReadBlock(buffer, 0, bufferSize) != 0)
                {
                    foreach (var character in buffer)
                    {
                        position++;
                        if (char.IsWhiteSpace(character))
                        {
                            if (character == '\n')
                            {
                                lineNumber++;
                                position = 0;
                            }
                            if (word.Length < 1)
                            {
                                continue;
                            }

                            var wordString = word.ToString();
                            yield return (wordString, new WordEntry(filePath, position - wordString.Length - 1, lineNumber));

                            word.Clear();
                        }
                        else if (!char.IsPunctuation(character))
                        {
                            word.Append(character);
                        }
                    }
                }
            }
        }

        //private void LoadFile(string filePath)
        //{
        //    var setOfWords = new HashSet<string>();  // direct index
        //    using (var reader = new StreamReader(filePath))
        //    {
        //        var node = _invertedIndex.Root;
        //        const int bufferSize = 2048;
        //        var buffer = new char[bufferSize];
        //        var position = 0;
        //        var lineNumber = 1;
        //        while (reader.ReadBlock(buffer, 0, bufferSize) != 0)
        //        {
        //            var wordBuilder = new StringBuilder();  // direct index
        //            foreach (var character in buffer)
        //            {
        //                position++;
        //                if (char.IsWhiteSpace(character))
        //                {
        //                    if (character == '\n')
        //                    {
        //                        position = 1;
        //                        lineNumber++;
        //                    }
        //                    if (node == _invertedIndex.Root)
        //                    {
        //                        continue;
        //                    }

        //                    var wordString = wordBuilder.ToString();
        //                    wordBuilder.Clear();

        //                    setOfWords.Add(wordString);

        //                    node.Lock.EnterWriteLock();
        //                    try
        //                    {
        //                        var wordEntry = new WordEntry(filePath, position - wordString.Length, lineNumber);
        //                        if (node.Files == null)
        //                        {
        //                            node.Files = new SortedSet<WordEntry> { wordEntry };
        //                        }
        //                        else
        //                        {
        //                            node.Files.Add(wordEntry);
        //                        }
        //                    }
        //                    finally
        //                    {
        //                        node.Lock.ExitWriteLock();
        //                    }

        //                    node = _invertedIndex.Root;
        //                }
        //                else if (!(char.IsPunctuation(character) && wordBuilder.Length == 0))
        //                {
        //                    wordBuilder.Append(character); 

        //                    PrefixTree<WordEntry>.Node newNode;
        //                    node.Lock.EnterWriteLock();
        //                    try
        //                    {
        //                        newNode = _invertedIndex.Add(character, node);
        //                    }
        //                    finally
        //                    {
        //                        node.Lock.ExitWriteLock();
        //                    }

        //                    node = newNode;
        //                }
        //            }
        //        }
        //    }

        //    _directIndex.Add(filePath, setOfWords); // direct index
        //}
    }
}