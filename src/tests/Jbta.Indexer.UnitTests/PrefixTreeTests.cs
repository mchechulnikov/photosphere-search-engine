using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
                trie.Add(word.ToCharArray(), "file");
            }

            Assert.True(trie.Contains("qiz"));
            Assert.True(trie.Contains("foo"));
            Assert.True(trie.Contains("fo"));
            Assert.True(trie.Contains("foob"));
            Assert.False(trie.Contains("invalid"));
            Assert.False(trie.Contains("fooc"));
        }

        [Fact]
        public void BigText()
        {
            var (trie, directIndex) = ReadPrefixTree();
            var test = trie.Get("Пьер");
            test = trie.Get("Пье");

            Assert.True(trie.Contains("что-нибудь"));
            Assert.True(trie.Contains("something"));
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
            Assert.True(trie.Contains("краковяк"));
            Assert.False(trie.Contains("кракозябры"));
        }

        private static (PrefixTree, IDictionary<string, ISet<string>>) ReadPrefixTree()
        {
            var directIndex = new Dictionary<string, ISet<string>>();  // direct index
            var trie = new PrefixTree();
            var filesPathes = GetFilesPathes("C:\\test-texts\\");
            Parallel.ForEach(filesPathes, (filePath, _, fileNumber) =>
            {
                var setOfWords = new HashSet<string>();  // direct index
                using (var reader = new StreamReader(filePath))
                {
                    var node = trie.Root;
                    const int bufferSize = 2048;
                    var buffer = new char[bufferSize];
                    while (reader.ReadBlock(buffer, 0, bufferSize) != 0)
                    {
                        var stringBuilder = new StringBuilder();  // direct index
                        foreach (var character in buffer)
                        {
                            if (char.IsWhiteSpace(character))
                            {
                                if (node == trie.Root)
                                {
                                    continue;
                                }
                                if (node.Files == null)
                                {
                                    node.Files = new HashSet<string>{ filePath };
                                }
                                else
                                {
                                    node.Files.Add(filePath);
                                }
                                node = trie.Root;
                                setOfWords.Add(stringBuilder.ToString()); // direct index
                                stringBuilder.Clear(); // direct index
                            }
                            else
                            {
                                stringBuilder.Append(character); // direct index
                                node = trie.Add(character, node);
                            }
                        }
                    }
                }

                directIndex.Add(filePath, setOfWords); // direct index
            });
            return (trie, directIndex);
        }

        private static IEnumerable<string> GetFilesPathes(string targetDirectory)
        {
            var directoryInfo = new DirectoryInfo(targetDirectory);
            foreach (var file in directoryInfo.EnumerateFiles())
            {
                yield return string.Intern(file.FullName);
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