using System;
using System.IO;
using System.Threading.Tasks;
using Photosphere.SearchEngine.IntegrationTests.Utils;
using Xunit;

namespace Photosphere.SearchEngine.IntegrationTests.SearchEngineTests.EventsTests
{
    public class FilePathChangedEventTests
    {
        [Fact]
        public async void FilePathChanged_FileRenamed_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var file = new TestFile("foo"))
            {
                engine.Add(file.Path);
                await Task.Delay(100);

                var newName = $"new-test-{Guid.NewGuid()}.txt";

                engine.FilePathChanged += args =>
                {
                    tcs.TrySetResult(Path.GetFullPath(".\\" + newName) == args.Path);
                };

                file.Rename(newName);

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void FilePathChanged_FileInNotWatchedFolderRenamed_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var file = new TestFile("foo", folderPath))
                {
                    engine.Add(file.Path);
                    await Task.Delay(100);

                    var newName = $"new-test-{Guid.NewGuid()}.txt";

                    engine.FilePathChanged += args =>
                    {
                        var expectedPath = Path.GetFullPath($".\\{folderPath}\\{newName}");
                        tcs.TrySetResult(expectedPath == args.Path);
                    };

                    file.Rename(newName);

                    Assert.True(await tcs.Task);
                }
            }
        }

        [Fact]
        public async void FilePathChanged_FileInWatchedFolderRenamed_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var file = new TestFile("foo", folderPath))
                {
                    engine.Add(folderPath);
                    await Task.Delay(100);

                    var newName = $"new-test-{Guid.NewGuid()}.txt";

                    engine.FilePathChanged += args =>
                    {
                        var expectedPath = Path.GetFullPath($".\\{folderPath}\\{newName}");
                        tcs.TrySetResult(expectedPath == args.Path);
                    };

                    file.Rename(newName);

                    Assert.True(await tcs.Task);
                }
            }
        }

        [Fact]
        public async void FilePathChanged_NotEmptyWatchedFolderRenamed_Raised()
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (new TestFile("foo", folderPath))
                {
                    engine.Add(folderPath);
                    await Task.Delay(100);

                    var newName = $"new-test-{Guid.NewGuid()}";

                    engine.FilePathChanged += args =>
                    {
                        var expectedPath = Path.GetFullPath($".\\{newName}");
                        tcs.TrySetResult(expectedPath == args.Path);
                    };

                    folder.Rename(newName);

                    Assert.True(await tcs.Task);
                }
            }
        }
    }
}