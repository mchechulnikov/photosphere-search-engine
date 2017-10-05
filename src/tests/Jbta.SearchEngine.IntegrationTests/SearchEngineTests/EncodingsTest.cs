using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests
{
    public class EncodingsTest
    {
        [Fact]
        public void Add_Utf8_SuccessfullyRecognized()
        {
            TestEncoding(TestFiles.Utf8, "текст", 1, 10);
        }

        [Fact]
        public void Add_Utf16_SuccessfullyRecognized()
        {
            TestEncoding(TestFiles.Utf16, "текст", 1, 10);
        }

        [Fact]
        public void Add_Utf16be_SuccessfullyRecognized()
        {
            TestEncoding(TestFiles.Utf16Be, "текст", 1, 10);
        }

        [Fact]
        public void Add_Windows1251_SuccessfullyRecognized()
        {
            TestEncoding(TestFiles.Windows1251, "текст", 1, 10);
        }

        [Fact]
        public void Add_Windows1252_SuccessfullyRecognized()
        {
            TestEncoding(TestFiles.Windows1252, "text", 1, 6);
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

        private static class TestFiles
        {
            private const string TextsDirectoryPath = ".\\test-texts\\EncodingTests\\";
            public static readonly string Utf8 = $"{TextsDirectoryPath}UTF-8.txt";
            public static readonly string Utf16 = $"{TextsDirectoryPath}UTF-16.txt";
            public static readonly string Utf16Be = $"{TextsDirectoryPath}UTF-16BE.txt";
            public static readonly string Windows1251 = $"{TextsDirectoryPath}Windows-1251.txt";
            public static readonly string Windows1252 = $"{TextsDirectoryPath}Windows-1252.txt";
        }
    }
}