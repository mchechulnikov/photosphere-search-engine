using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jbta.Indexing.Indexing;
using Xunit;

namespace Jbta.Indexing.UnitTests
{
    public class PrefixTreeTests
    {
        [Fact]
        public void SmallText()
        {
            const string str = "foo bar qiz foobar";
            var trie = new PrefixTree();
            foreach (var word in str.Split(' '))
            {
                trie.Add(word.ToCharArray());
            }

            Assert.True(trie.Contains("qiz"));
            Assert.True(trie.Contains("foo"));
            Assert.True(trie.Contains("fo"));
            Assert.True(trie.Contains("foob"));
            Assert.False(trie.Contains("invalid"));
            Assert.False(trie.Contains("fooc"));
        }

        [Fact]
        public void BigText0()
        {
            var trie = ReadPrefixTree();
            Assert.True(trie.Contains("War"));
            Assert.True(trie.Contains("war"));
            Assert.True(trie.Contains("Peace"));
            Assert.True(trie.Contains("peace"));
            Assert.True(trie.Contains("pe"));
            Assert.True(trie.Contains("Pier"));
            Assert.True(trie.Contains("Natasha"));
            Assert.True(trie.Contains("Пьер"));
            Assert.True(trie.Contains("Пьер"));
            Assert.True(trie.Contains("Наташа"));
            Assert.True(trie.Contains("Раскольников"));
        }

        private static PrefixTree ReadPrefixTree()
        {
            var trie = new PrefixTree();
            var filesPathes = GetFilesPathes("test-texts\\");
            Parallel.ForEach(filesPathes, (filePath, _, fileNumber) =>
            {
                using (var reader = new StreamReader(filePath))
                {
                    var node = trie.Root;
                    const int bufferSize = 2048;
                    var buffer = new char[bufferSize];
                    while (reader.ReadBlock(buffer, 0, bufferSize) != 0)
                    {
                        foreach (var symbol in buffer)
                        {
                            if (symbol == ' ' || symbol == '\n' || symbol == '\r')
                            {
                                if (node == trie.Root)
                                {
                                    continue;
                                }
                                node.IsTerminal = true;
                                node = trie.Root;
                            }
                            else
                            {
                                node = trie.Add(symbol, false, node);
                            }
                        }
                    }
                }
            });
            return trie;
        }

        private static IEnumerable<string> GetFilesPathes(string targetDirectory)
        {
            var directoryInfo = new DirectoryInfo(targetDirectory);
            foreach (var file in directoryInfo.EnumerateFiles())
            {
                yield return file.FullName;
            }

            foreach (var subdirectory in directoryInfo.EnumerateDirectories())
            {
                foreach (var filePath in GetFilesPathes(subdirectory.FullName))
                {
                    yield return filePath;
                }
            }
        }
    }
}