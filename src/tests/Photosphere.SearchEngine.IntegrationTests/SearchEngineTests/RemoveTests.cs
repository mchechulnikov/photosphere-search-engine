using Photosphere.SearchEngine.IntegrationTests.Resources;
using Xunit;

namespace Photosphere.SearchEngine.IntegrationTests.SearchEngineTests
{
    public class RemoveTests
    {
        [Fact]
        public void Remove_EmptyPath_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Remove(string.Empty));
        }

        [Fact]
        public void Remove_NullPath_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Remove(null));
        }

        [Fact]
        public void Remove_InvalidPath_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Remove("INVALID"));
        }

        [Fact]
        public void Remove_FilePathNotExists_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Remove(TestTextFiles.OneLineFile + "foo"));
        }

        [Fact]
        public void Remove_DirectoryPathNotExists_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Remove(TestTextFiles.RootDirectory + "foo"));
        }

        [Fact]
        public void Remove_RemoveAdded_ReturnsTrue()
        {
            var engine = SearchEngineFactory.New();
            engine.Add(TestTextFiles.OneLineFile);
            Assert.True(engine.Remove(TestTextFiles.OneLineFile));
        }

        [Fact]
        public void Remove_NotAddedFile_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Remove(TestTextFiles.OneLineFile));
        }
    }
}