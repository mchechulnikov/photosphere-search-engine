using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jbta.SearchEngine.FileParsing
{
    internal class StandartFileParser : IFileParser
    {
        public IEnumerable<(string word, WordEntry)> Parse(string filePath)
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
        //        var node = _searchIndex.Root;
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
        //                    if (node == _searchIndex.Root)
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

        //                    node = _searchIndex.Root;
        //                }
        //                else if (!(char.IsPunctuation(character) && wordBuilder.Length == 0))
        //                {
        //                    wordBuilder.Append(character); 

        //                    PrefixTree<WordEntry>.Node newNode;
        //                    node.Lock.EnterWriteLock();
        //                    try
        //                    {
        //                        newNode = _searchIndex.Add(character, node);
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