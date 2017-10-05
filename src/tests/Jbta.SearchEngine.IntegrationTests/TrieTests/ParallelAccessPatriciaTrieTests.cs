using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.Index.Trie;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.TrieTests
{
    public class ParallelAccessPatriciaTrieTests
    {
        [Fact]
        public void Get_ToOneNode_Success()
        {
            var trie = GetTrieWithData();

            var sourceTasks = new []
            {
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
                new Task<int>(() => trie.Get("арбуз").First()),
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
            var trie = GetTrieWithData();

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

        [Fact]
        public void Add_WhileReading_Success()
        {
            var trie = GetTrieWithData();

            var tasksGet = new[]
            {
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
                new Task<int>(() => trie.Get("ар").First()),
            };

            var randomizer = new Random();
            var data = new Dictionary<string, int>()
            {
                {"арбузfoo", randomizer.Next()},
                {"арбузbar", randomizer.Next()},
                {"арбузbuz", randomizer.Next()},
                {"арбузqiz", randomizer.Next()},
                {"арбузfoobar", randomizer.Next()}
            };
            var tasksAdd = new[]
            {
                new Task(() => trie.Add("арбузfoo", data["арбузfoo"])),
                new Task(() => trie.Add("арбузbar", data["арбузbar"])),
                new Task(() => trie.Add("арбузbuz", data["арбузbuz"])),
                new Task(() => trie.Add("арбузqiz", data["арбузqiz"])),
                new Task(() => trie.Add("арбузfoobar", data["арбузfoobar"]))
            };
            var tasks = tasksGet.Union(tasksAdd).ToArray();

            foreach (var task in tasks) task.Start();
            Task.WaitAll(tasks);

            // check get method
            var result = tasksGet.First().Result;
            foreach (var task in tasksGet)
            {
                Assert.Equal(result, task.Result);
            }

            // check add method
            foreach (var kv in data)
            {
                Assert.Equal(kv.Value, trie.Get(kv.Key).First());
            }
        }

        [Fact]
        public void Remove_WhileAdding_Success()
        {
            var trie = GetTrieWithData();
            var randomizer = new Random();

            trie.Add("арбузfordel1", randomizer.Next());
            trie.Add("арбузfordel2", randomizer.Next());
            trie.Add("арбузfordel3", randomizer.Next());
            trie.Add("арбузfordel4", randomizer.Next());
            trie.Add("арбузfordel5", randomizer.Next());

            var data = new Dictionary<string, int>
            {
                {"арбузfoo", randomizer.Next()},
                {"арбузbar", randomizer.Next()},
                {"арбузbuz", randomizer.Next()},
                {"арбузqiz", randomizer.Next()},
                {"арбузfoobar", randomizer.Next()}
            };
            var tasksAdd = new[]
            {
                new Task(() => trie.Add("арбузfoo", data["арбузfoo"])),
                new Task(() => trie.Add("арбузbar", data["арбузbar"])),
                new Task(() => trie.Add("арбузbuz", data["арбузbuz"])),
                new Task(() => trie.Add("арбузqiz", data["арбузqiz"])),
                new Task(() => trie.Add("арбузfoobar", data["арбузfoobar"]))
            };
            var taskRemove = new[]
            {
                new Task(() => trie.Remove("арбузfordel1", i => true)),
                new Task(() => trie.Remove("арбузfordel2", i => true)),
                new Task(() => trie.Remove("арбузfordel3", i => true)),
                new Task(() => trie.Remove("арбузfordel4", i => true)),
                new Task(() => trie.Remove("арбузfordel5", i => true))
            };

            var tasks = tasksAdd.Union(taskRemove).ToArray();

            foreach (var task in tasks) task.Start();
            Task.WaitAll(tasks);

            // check remove method
            foreach (var i in Enumerable.Range(1, 5))
            {
                Assert.Equal(0, trie.Get("арбузfordel" + i).Count());
            }

            // check add method
            foreach (var kv in data)
            {
                Assert.Equal(kv.Value, trie.Get(kv.Key).First());
            }
        }

        private static ITrie<int> GetTrieWithData()
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

            return trie;
        }
    }
}