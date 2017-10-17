using System.IO;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Utils;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests.EventsTests
{
    public class PathWatchingStartedEventTests
    {
        [Fact]
        public async void PathWatchingStarted_FileAdded_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var file = new TestFile("foo"))
            {
                var filePath = file.Path;

                engine.PathWatchingStarted += args =>
                {
                    tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                };

                engine.Add(filePath);
                await Task.Delay(100);

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void PathWatchingStarted_EmptyFolderAdded_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;

                engine.PathWatchingStarted += args =>
                {
                    tcs.TrySetResult(Path.GetFullPath(folderPath) == args.Path);
                };

                engine.Add(folderPath);
                await Task.Delay(100);

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void PathWatchingStarted_FolderWithFileAdded_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (new TestFile("foo", folderPath))
                {
                    engine.PathWatchingStarted += args =>
                    {
                        tcs.TrySetResult(Path.GetFullPath(folderPath) == args.Path);
                    };

                    engine.Add(folderPath);
                    await Task.Delay(100);

                    Assert.True(await tcs.Task);
                }
            }
        }

        [Fact]
        public async void PathWatchingStarted_FolderWithFileAddedAgain_NotRaised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                using (new TestFile("foo", folder.Path))
                {
                    engine.Add(folder.Path);
                    await Task.Delay(100);
                }
            }

            engine.PathWatchingStarted += args =>
            {
                tcs.TrySetResult(false);
            };

            #pragma warning disable CS4014
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                tcs.TrySetResult(true);
            });
            #pragma warning restore CS4014

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (new TestFile("foo", folderPath))
                {
                    Assert.True(await tcs.Task);
                }
            }
        }
    }
}