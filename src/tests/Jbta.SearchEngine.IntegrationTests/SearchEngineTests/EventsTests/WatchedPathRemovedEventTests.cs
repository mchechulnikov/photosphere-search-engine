using System.IO;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Utils;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests.EventsTests
{
    public class WatchedPathRemovedEventTests
    {
        [Fact]
        public async void WatchedPathRemoved_WatchedFileRemoved_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var file = new TestFile("foo"))
            {
                var filePath = file.Path;
                engine.Add(filePath);
                await Task.Delay(100);

                engine.WatchedPathRemoved += args =>
                {
                    tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                };

                file.Delete();

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void WatchedPathRemoved_NotEmptyWatchedFolderRemoved_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (new TestFile("foo", folderPath))
                {
                    engine.Add(folder.Path);
                    await Task.Delay(300);

                    engine.WatchedPathRemoved += args =>
                    {
                        tcs.TrySetResult(Path.GetFullPath(folderPath) == args.Path);
                    };

                    folder.Delete();

                    Assert.True(await tcs.Task);
                }
            }
        }

        [Fact]
        public async void WatchedPathRemoved_EmptyWatchedFolderRemoved_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var file = new TestFile("foo", folderPath))
                {
                    engine.Add(folder.Path);
                    await Task.Delay(100);

                    engine.WatchedPathRemoved += args =>
                    {
                        tcs.TrySetResult(Path.GetFullPath(folderPath) == args.Path);
                    };

                    file.Delete();

                    folder.Delete();

                    Assert.True(await tcs.Task);
                }
            }
        }
    }
}