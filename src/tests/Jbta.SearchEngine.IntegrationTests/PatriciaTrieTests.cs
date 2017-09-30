using System;
using System.Collections.Generic;
using System.Linq;
using Jbta.SearchEngine.Index;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests
{
    public class PatriciaTrieTests
    {
        [Fact]
        public void SimpleWordsWithNumbers_JustAdded_SuccessedRetrieved()
        {
            var randomizer = new Random();
            var words = new Dictionary<string, int>
            {
                { "арбуз", randomizer.Next() },
                { "аркебуза", randomizer.Next() },
                { "арбузяка", randomizer.Next() },
                { "арбузка", randomizer.Next() },
                { "арбузяшка", randomizer.Next() },
                { "арлекин", randomizer.Next() }
            };
            var trie = new PatriciaTrie<int>();

            foreach (var kv in words)
            {
                trie.Add(kv.Key, kv.Value);
            }

            foreach (var kv in words)
            {
                Assert.Equal(kv.Value, trie.Get(kv.Key).First());
            }
        }
    }
}