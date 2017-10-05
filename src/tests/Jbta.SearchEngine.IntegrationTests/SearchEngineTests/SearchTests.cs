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

        [Theory]
        [InlineData("Кутузов", 0)]
        [InlineData("Кутузова", 2)]
        [InlineData("князь", 40)]
        [InlineData("Наташа", 0)]
        [InlineData("что", 101)]
        public void Search_WholeWord_ExpectedEntriesCount(string query, int cout)
        {
            IndexFileAndDoAction(TestTextFiles.WarAndPeace1, (engine, filePath) =>
            {
                var wordEntries = engine.Search(query, true).ToList();

                Assert.Equal(cout, wordEntries.Count);
            });
        }

        [Theory]
        [InlineData("Кутузов", 1)]
        [InlineData("князь", 1)]
        [InlineData("Наташа", 0)]
        [InlineData("что", 1)]
        public void SearchFiles_ByPrefix_ExpectedFilesCount(string query, int cout)
        {
            IndexFileAndDoAction(TestTextFiles.WarAndPeace1, (engine, filePath) =>
            {
                var wordEntries = engine.SearchFiles(query).ToList();

                Assert.Equal(cout, wordEntries.Count);
            });
        }

        [Theory]
        [InlineData("Кутузов", 0)]
        [InlineData("Кутузова", 1)]
        [InlineData("князь", 1)]
        [InlineData("Наташа", 0)]
        [InlineData("что", 1)]
        public void SearchFiles_WholeWord_ExpectedFilesCount(string query, int cout)
        {
            IndexFileAndDoAction(TestTextFiles.WarAndPeace1, (engine, filePath) =>
            {
                var wordEntries = engine.SearchFiles(query, true).ToList();

                Assert.Equal(cout, wordEntries.Count);
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