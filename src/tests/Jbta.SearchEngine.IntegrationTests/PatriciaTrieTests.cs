using System;
using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Trie;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests
{
    public class PatriciaTrieTests
    {
        [Fact]
        public void Get_SimpleWordsWithNumbers_SuccessedRetrieved()
        {
            var (trie, map) = GetTrieWithData();

            foreach (var kv in map)
            {
                Assert.Equal(kv.Value, trie.Get(kv.Key).First());
            }
        }

        [Fact]
        public void Get_NotWholeWord_SuccessedRetrieved()
        {
            var (trie, map) = GetTrieWithData();
            var result = trie.Get("арбуз");
            Assert.Equal(map["арбуз"], result.First());
        }

        [Fact]
        public void Get_NotWholeWord_CorrectResultsNumber()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбузка");
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void Get_NotWholeWordPartOfExistingKey_ZeroResults()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбу");
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public void Get_WholeWord_SuccessedRetrieved()
        {
            var (trie, map) = GetTrieWithData();
            var result = trie.Get("арбуз", true);
            Assert.Equal(map["арбуз"], result.First());
        }

        [Fact]
        public void Get_WholeWord_CorrectResultsNumber()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбуз", true);
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void Get_WholeWordPartOfExistingKey_ZeroResults()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбу", true);
            Assert.Equal(0, result.Count());
        }

        private static (ITrie<int> trie, IDictionary<string, int> map) GetTrieWithData()
        {
            var randomizer = new Random();
            var words = new Dictionary<string, int>
            {
                {"арбуз", randomizer.Next()},
                {"аркебуза", randomizer.Next()},
                {"арбузяка", randomizer.Next()},
                {"арбузка", randomizer.Next()},
                {"арбузяшка", randomizer.Next()},
                {"арлекин", randomizer.Next()},
                {"аптека", randomizer.Next()},
            };
            var trie = new PatriciaTrie<int>();

            foreach (var kv in words)
            {
                trie.Add(kv.Key, kv.Value);
            }

            return (trie, words);
        }
    }
}