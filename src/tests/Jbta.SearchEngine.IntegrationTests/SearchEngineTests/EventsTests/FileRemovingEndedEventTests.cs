using System.IO;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Resources;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests.EventsTests
{
    public class FileRemovingEndedEventTests
    {
        [Fact]
        public async void FileRemovingEnded_FileRemoved_Raised()
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileRemovingEnded += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
            };

            Assert.True(engine.Add(filePath));
            Assert.True(engine.Remove(filePath));
            Assert.True(await tcs.Task);
        }
    }
}