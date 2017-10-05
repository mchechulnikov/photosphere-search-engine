using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Resources;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests
{
    public class EncodingsTest
    {
        [Fact]
        public void Add_Utf8_SuccessfullyRecognized()
        {
            TestEncoding(TestTextFiles.Utf8, "текст", 1, 10);
        }

        [Fact]
        public void Add_Utf16_SuccessfullyRecognized()
        {
            TestEncoding(TestTextFiles.Utf16, "текст", 1, 10);
        }

        [Fact]
        public void Add_Utf16be_SuccessfullyRecognized()
        {
            TestEncoding(TestTextFiles.Utf16Be, "текст", 1, 10);
        }

        [Fact]
        public void Add_Windows1251_SuccessfullyRecognized()
        {
            TestEncoding(TestTextFiles.Windows1251, "текст", 1, 10);
        }

        [Fact]
        public void Add_Windows1252_SuccessfullyRecognized()
        {
            TestEncoding(TestTextFiles.Windows1252, "text", 1, 6);
        }

        private static async void TestEncoding(string filePath, string searchQuery, int expectedLineNumber, int expectedPosition)
        {
            var engine = SearchEngineFactory.New();

            var tcs = new TaskCompletionSource<bool>();

            engine.FileIndexed += args =>
            {
                var wordEntries = engine.Search(searchQuery).ToList();

                Assert.Equal(1, wordEntries.Count);

                var entry = wordEntries.First();
                Assert.Equal(expectedLineNumber, entry.LineNumber);
                Assert.Equal(expectedPosition, entry.Position);

                tcs.TrySetResult(true);
            };

            var isAdded = engine.Add(filePath);
            Assert.True(isAdded);
            Assert.True(await tcs.Task);
        }
    }
}