using System;
using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Resources;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests
{
    public class SearchTests
    {
        [Theory]
        [InlineData("Кутузов", 5)]
        [InlineData("князь", 40)]
        [InlineData("Наташа", 0)]
        [InlineData("что", 119)]
        public void Search_ByPrefix_ExpectedEntriesCount(string query, int cout)
        {
            IndexFileAndDoAction(TestTextFiles.WarAndPeace1, (engine, filePath) =>
            {
                var wordEntries = engine.Search(query).ToList();

                Assert.Equal(cout, wordEntries.Count);
            });
        }

        [Theory]
        [InlineData("mon", 1, 12)]
        [InlineData("prince", 1, 16)]
        [InlineData("Как", 7, 3)]
        [InlineData("Австрию", 15, 27)]
        public void Search_ByPrefix_ExpectedEntriesPositions(string query, int lineNumber, int position)
        {
            IndexFileAndDoAction(TestTextFiles.WarAndPeace1, (engine, filePath) =>
            {
                var wordEntry = engine.Search(query).First();

                Assert.Equal(lineNumber, wordEntry.LineNumber);
                Assert.Equal(position, wordEntry.Position);
            });
        }

        private static async void IndexFileAndDoAction(string filePath, Action<ISearchEngine, string> action)
        {
            var engine = SearchEngineFactory.New();

            var tcs = new TaskCompletionSource<bool>();

            engine.FileIndexingEnded += args =>
            {
                action(engine, args.FilePath);
                tcs.TrySetResult(true);
            };

            var isAdded = engine.Add(filePath);
            Assert.True(isAdded);
            Assert.True(await tcs.Task);
        }
    }
}