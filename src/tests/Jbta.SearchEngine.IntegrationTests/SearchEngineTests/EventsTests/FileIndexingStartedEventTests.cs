using System.IO;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Resources;
using Jbta.SearchEngine.IntegrationTests.Utils;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests.EventsTests
{
    public class FileIndexingStartedEventTests
    {
        [Fact]
        public async void FileIndexingStarted_FileAdded_Raised()
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileIndexingStarted += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
            };

            Assert.True(engine.Add(filePath));
            Assert.True(await tcs.Task);
        }

        [Fact]
        public async void FileIndexingStarted_FileChanged_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var file = new TestFile("foo"))
            {
                var filePath = file.Path;

                engine.Add(filePath);
                await Task.Delay(100);

                engine.FileIndexingStarted += args =>
                {
                    tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                };

                file.ChangeContent("bar");

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void FileIndexingStarted_EmptyFolderIndexed_NotRaised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;

                #pragma warning disable CS4014
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    tcs.TrySetResult(true);
                });
                #pragma warning restore CS4014

                engine.Add(folderPath);
                await Task.Delay(100);

                engine.FileIndexingStarted += args =>
                {
                    tcs.TrySetResult(false);
                };

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void FileIndexingStarted_FileCreatedToWatchedDirectory_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var fileName = TestFile.GenerateFileName();
                var folderPath = folder.Path;

                engine.Add(folderPath);
                await Task.Delay(100);

                engine.FileIndexingStarted += args =>
                {
                    tcs.TrySetResult(Path.GetFullPath($"{folderPath}\\{fileName}") == args.Path);
                };

                using (new TestFile("foo", folderPath, fileName))
                {
                    Assert.True(await tcs.Task);
                }
            }
        }

        [Fact]
        public async void FileIndexingStarted_FileMovedToWatchedDirectory_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var sourceFolder = new TestFolder())
            {
                using (var file = new TestFile("foo", sourceFolder.Path))
                {
                    var fileName = file.Name;
                    using (var destFolder = new TestFolder())
                    {
                        var destFolderPath = destFolder.Path;
                        using (new TestFile("bar", destFolderPath))
                        {
                            engine.Add(destFolderPath);
                            await Task.Delay(100);

                            engine.FileIndexingStarted += args =>
                            {
                                tcs.TrySetResult(Path.GetFullPath($"{destFolderPath}\\{fileName}") == args.Path);
                            };

                            file.Move(destFolderPath);

                            Assert.True(await tcs.Task);
                        }
                    }
                }
            }
        }
    }
}