using System.IO;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Resources;
using Jbta.SearchEngine.IntegrationTests.Utils;
using Jbta.SearchEngine.IntegrationTests.Utils.Extensions;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests.EventsTests
{
    public class FileRemovingEventsTests
    {
        [Theory]
        [InlineData(nameof(ISearchEngine.FileRemovingStarted))]
        [InlineData(nameof(ISearchEngine.FileRemovingEnded))]
        public async void FileRemovingEvents_FileRemoved_Raised(string eventName)
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.AddHandler(eventName, args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
            });

            Assert.True(engine.Add(filePath));
            Assert.True(engine.Remove(filePath));
            Assert.True(await tcs.Task);
        }

        [Theory]
        [InlineData(nameof(ISearchEngine.FileRemovingStarted))]
        [InlineData(nameof(ISearchEngine.FileRemovingEnded))]
        public async void FileRemovingEvents_WatchedFolderWithFileRemoved_Raised(string eventName)
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var file = new TestFile("foo", folderPath))
                {
                    var filePath = file.Path;

                    engine.Add(folderPath);
                    await Task.Delay(100);

                    engine.AddHandler(eventName, args =>
                    {
                        tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                    });

                    folder.Delete();

                    Assert.True(await tcs.Task);
                }
            }
        }

        [Theory]
        [InlineData(nameof(ISearchEngine.FileRemovingStarted))]
        [InlineData(nameof(ISearchEngine.FileRemovingEnded))]
        public async void FileRemovingEvents_FolderWithFileRemoved_Raised(string eventName)
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var subFolder = new TestFolder(folderPath))
                {
                    var subFolderPath = subFolder.Path;
                    using (var file = new TestFile("foo", subFolderPath))
                    {
                        var filePath = file.Path;

                        engine.Add(folderPath);
                        await Task.Delay(100);

                        engine.AddHandler(eventName, args =>
                        {
                            tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                        });

                        subFolder.Delete();

                        Assert.True(await tcs.Task);
                    }
                }
            }
        }

        [Theory]
        [InlineData(nameof(ISearchEngine.FileRemovingStarted))]
        [InlineData(nameof(ISearchEngine.FileRemovingEnded))]
        public async void FileRemovingEvents_FileInFolderRemoved_Raised(string eventName)
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var subFolder = new TestFolder(folderPath))
                {
                    var subFolderPath = subFolder.Path;
                    using (var file = new TestFile("foo", subFolderPath))
                    {
                        var filePath = file.Path;

                        engine.Add(folderPath);
                        await Task.Delay(100);

                        engine.AddHandler(eventName, args =>
                        {
                            tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                        });

                        file.Delete();

                        Assert.True(await tcs.Task);
                    }
                }
            }
        }

        [Theory]
        [InlineData(nameof(ISearchEngine.FileRemovingStarted))]
        [InlineData(nameof(ISearchEngine.FileRemovingEnded))]
        public async void FileRemovingEvents_FileMovedToFolder_Raised(string eventName)
        {
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            using (var folder = new TestFolder())
            {
                var folderPath = folder.Path;
                using (var file = new TestFile("foo", folderPath))
                {
                    var filePath = file.Path;
                    using (var subFolder = new TestFolder(folderPath))
                    {
                        var subFolderPath = subFolder.Path;

                        engine.Add(folderPath);
                        await Task.Delay(100);

                        engine.AddHandler(eventName, args =>
                        {
                            tcs.TrySetResult(Path.GetFullPath(filePath) == args.Path);
                        });

                        file.Move(subFolderPath);

                        Assert.True(await tcs.Task);
                    }
                }
            }
        }
    }
}