using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.Index.Trie;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.IndexTests.TrieTests
{
    public class ParallelAccessPatriciaTrieTests
    {
        [Fact]
        public void Get_ToOneNode_Success()
        {
            var (trie, _) = GetTrieWithData();

            var sourceTasks = new []
            {
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First())
            };
            var tasks = sourceTasks.Cast<Task>().ToArray();

            foreach (var task in tasks) task.Start();
            Task.WaitAll(tasks);

            var result = sourceTasks.First().Result;
            foreach (var task in sourceTasks)
            {
                Assert.Equal(result, task.Result);
            }
        }

        [Fact]
        public void Get_ToParentAndChildNode_Success()
        {
            var (trie, _) = GetTrieWithData();

            var tasksToParent = new[]
            {
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
            };
            var tasksToChild = new[]
            {
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
            };
            var tasks = tasksToParent.Union(tasksToChild).Cast<Task>().ToArray();

            foreach (var task in tasks) task.Start();
            Task.WaitAll(tasks);

            var result = tasksToParent.First().Result;
            foreach (var task in tasksToParent)
            {
                Assert.Equal(result, task.Result);
            }
            result = tasksToChild.First().Result;
            foreach (var task in tasksToChild)
            {
                Assert.Equal(result, task.Result);
            }
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