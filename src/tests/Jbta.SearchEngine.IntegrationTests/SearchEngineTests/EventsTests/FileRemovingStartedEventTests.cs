using System.IO;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Resources;
using Jbta.SearchEngine.IntegrationTests.Utils;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests.EventsTests
{
    public class FileRemovingStartedEventTests
    {
        [Fact]
        public async void FileRemovingStarted_FileRemoved_Raised()
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileRemovingStarted += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
            };

            Assert.True(engine.Add(filePath));
            Assert.True(engine.Remove(filePath));
            Assert.True(await tcs.Task);
        }

        [Fact]
        public async void FileRemovingStarted_WatchedFolderWithFileRemoved_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var file = new TestFile("foo", folderPath))
                {
                    var filePath = file.Path;

                    engine.Add(folder.Path);
                    await Task.Delay(100);

                    engine.FileRemovingStarted += args =>
                    {
                        tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                    };

                    folder.Delete();

                    Assert.True(await tcs.Task);
                }
            }
        }
    }
}