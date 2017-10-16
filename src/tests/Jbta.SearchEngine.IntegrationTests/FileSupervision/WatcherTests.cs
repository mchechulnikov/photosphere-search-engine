using System.IO;
using Jbta.SearchEngine.IntegrationTests.Utils;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.FileSupervision
{
    public class WatcherTests
    {
        [Fact]
        public void Rename()
        {
            using (var folder = new TestFolder())
            {
                var lastWriteTimeUtc = folder.LastWriteTimeUtc;
                folder.Rename("foo");
                Assert.Equal(lastWriteTimeUtc, folder.LastWriteTimeUtc);
            }
        }

        [Fact]
        public void FileAdded()
        {
            using (var folder = new TestFolder())
            {
                var lastWriteTimeUtc = folder.LastWriteTimeUtc;
                using (new TestFile("foo", folder.Path))
                {
                    Assert.NotEqual(lastWriteTimeUtc, folder.LastWriteTimeUtc);
                }
            }
        }
    }
}