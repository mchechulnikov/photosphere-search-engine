using System;
using System.Collections.Generic;
using System.Linq;
using Photosphere.SearchEngine.Index.Trie;
using Xunit;

namespace Photosphere.SearchEngine.IntegrationTests.TrieTests
{
    public class PatriciaTrieTests
    {
        [Fact]
        public void Get_SimpleWordsWithNumbers_SuccessedRetrieveValues()
        {
            var (trie, map) = GetTrieWithData();

            foreach (var kv in map)
            {
                Assert.Equal(kv.Value, Enumerable.First<int>(trie.Get(kv.Key)));
            }
        }

        [Fact]
        public void Get_NotWholeWord_SuccessedRetrieveValue()
        {
            var (trie, map) = GetTrieWithData();
            var result = trie.Get("арбуз");
            Assert.Equal(map["арбуз"], Enumerable.First<int>(result));
        }

        [Fact]
        public void Get_NotWholeWord_CorrectResultsNumber()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбузка");
            Assert.Equal(1, Enumerable.Count<int>(result));
        }

        [Fact]
        public void Get_NotWholeWordPartOfExistingKey_ZeroResults()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбу");
            Assert.Equal(4, Enumerable.Count<int>(result));
        }

        [Fact]
        public void Get_WholeWord_SuccessedRetrieved()
        {
            var (trie, map) = GetTrieWithData();
            var result = trie.Get("арбуз", true);
            Assert.Equal(map["арбуз"], Enumerable.First<int>(result));
        }

        [Fact]
        public void Get_WholeWord_CorrectResultsNumber()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбуз", true);
            Assert.Equal(1, Enumerable.Count<int>(result));
        }

        [Fact]
        public void Get_WholeWordPartOfExistingKey_ZeroResults()
        {
            var (trie, _) = GetTrieWithData();
            var result = trie.Get("арбу", true);
            Assert.Equal(0, Enumerable.Count<int>(result));
        }

        [Fact]
        public void Add_OneKeyTwice_TwoResultValues()
        {
            var trie = new PatriciaTrie<int>();

            trie.Add("foo", 42);
            trie.Add("foo", 43);

            var result = trie.Get("foo");
            Assert.Equal(2, Enumerable.Count<int>(result));
        }

        [Fact]
        public void Add_OneKeyTwice_ExpectedValues()
        {
            var trie = new PatriciaTrie<int>();

            trie.Add("foo", 42);
            trie.Add("foo", 43);

            var result = Enumerable.ToArray<int>(trie.Get("foo"));
            Assert.Equal(42, result[0]);
            Assert.Equal(43, result[1]);
        }

        [Fact]
        public void Remove_OneKeyOneValue_Removed()
        {
            var trie = new PatriciaTrie<int>();
            trie.Add("foo", 42);

            trie.Remove("foo", v => true);

            Assert.Equal(0, Enumerable.Count<int>(trie.Get("foo")));
        }

        [Fact]
        public void Remove_OneKeyTwoValuesRemoveBoth_Removed()
        {
            var trie = new PatriciaTrie<int>();
            trie.Add("foo", 42);
            trie.Add("foo", 43);

            trie.Remove("foo", v => true);

            Assert.Equal(0, Enumerable.Count<int>(trie.Get("foo")));
        }

        [Fact]
        public void Remove_OneKeyTwoValuesRemoveOne_OneValueLost()
        {
            var trie = new PatriciaTrie<int>();
            trie.Add("foo", 42);
            trie.Add("foo", 43);

            trie.Remove("foo", v => v == 42);

            Assert.Equal(1, Enumerable.Count<int>(trie.Get("foo")));
        }

        [Fact]
        public void Remove_OneKeyTwoValuesRemoveOne_ExpectedValueLost()
        {
            var trie = new PatriciaTrie<int>();
            trie.Add("foo", 42);
            trie.Add("foo", 43);

            trie.Remove("foo", v => v == 42);

            Assert.Equal(43, Enumerable.First<int>(trie.Get("foo")));
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