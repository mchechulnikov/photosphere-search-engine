using System;
using System.IO;
using System.Threading.Tasks;
using Jbta.SearchEngine.IntegrationTests.Resources;
using Jbta.SearchEngine.IntegrationTests.Utils;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests
{
    public class EventsTests
    {
        [Fact]
        public async void FileIndexingStarted_FileAdded_Raised()
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileIndexingStarted += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.FilePath);
            };

            Assert.True(engine.Add(filePath));
            Assert.True(await tcs.Task);
        }

        [Fact]
        public async void FileIndexingEnded_FileAdded_Raised()
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileIndexingEnded += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.FilePath);
            };

            Assert.True(engine.Add(filePath));
            Assert.True(await tcs.Task);
        }

        [Fact]
        public async void FileRemovingStarted_FileRemoved_Raised()
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileRemovingStarted += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.FilePath);
            };

            Assert.True(engine.Add(filePath));
            Assert.True(engine.Remove(filePath));
            Assert.True(await tcs.Task);
        }

        [Fact]
        public async void FileRemovingEnded_FileRemoved_Raised()
        {
            var filePath = TestTextFiles.OneLineFile;
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileRemovingEnded += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(filePath) == args.FilePath);
            };

            Assert.True(engine.Add(filePath));
            Assert.True(engine.Remove(filePath));
            Assert.True(await tcs.Task);
        }

        [Fact]
        public async void FileUpdateInitiated_FileChanged_Raised()
        {
            var testFilePath = $".\\test-{Guid.NewGuid()}.txt";
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileUpdateInitiated += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(testFilePath) == args.FilePath);
            };

            using (var file = new TestFile(testFilePath, "foo"))
            {
                engine.Add(file.Path);
                await Task.Delay(100);

                file.ChangeContent("bar");

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void FileIndexingStarted_FileChanged_Raised()
        {
            var testFilePath = $".\\test-{Guid.NewGuid()}.txt";
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FileIndexingStarted += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(testFilePath) == args.FilePath);
            };

            using (var file = new TestFile(testFilePath, "foo"))
            {
                engine.Add(file.Path);
                await Task.Delay(100);

                file.ChangeContent("bar");

                Assert.True(await tcs.Task);
            }
        }

        [Fact]
        public async void FilePathChanged_FileRenamed_Raised()
        {
            var oldTestFilePath = $".\\old-test-{Guid.NewGuid()}.txt";
            var newTestFileName = $"new-test-{Guid.NewGuid()}.txt";
            var engine = SearchEngineFactory.New();
            var tcs = new TaskCompletionSource<bool>();

            engine.FilePathChanged += args =>
            {
                tcs.TrySetResult(Path.GetFullPath(".\\" + newTestFileName) == args.FilePath);
            };

            using (var file = new TestFile(oldTestFilePath, "foo"))
            {
                engine.Add(file.Path);
                await Task.Delay(100);

                file.Rename(newTestFileName);

                Assert.True(await tcs.Task);
            }
        }
    }
}