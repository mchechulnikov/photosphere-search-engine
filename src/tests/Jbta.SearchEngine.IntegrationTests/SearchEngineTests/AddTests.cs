using Jbta.SearchEngine.IntegrationTests.Resources;
using Xunit;

namespace Jbta.SearchEngine.IntegrationTests.SearchEngineTests
{
    public class AddTests
    {
        [Fact]
        public void Add_EmptyPath_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Add(string.Empty));
        }

        [Fact]
        public void Add_NullPath_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Add(null));
        }

        [Fact]
        public void Add_InvalidPath_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Add("INVALID"));
        }

        [Fact]
        public void Add_FilePathNotExists_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Add(TestTextFiles.OneLineFile + "foo"));
        }

        [Fact]
        public void Add_DirectoryPathNotExists_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            Assert.False(engine.Add(TestTextFiles.RootDirectory + "foo"));
        }

        [Fact]
        public void Add_OneFile_ReturnsTrue()
        {
            var engine = SearchEngineFactory.New();
            Assert.True(engine.Add(TestTextFiles.OneLineFile));
        }

        [Fact]
        public void Add_TwoFiles_ReturnsTrue()
        {
            var engine = SearchEngineFactory.New();
            Assert.True(engine.Add(TestTextFiles.OneLineFile));
            Assert.True(engine.Add(TestTextFiles.TwoLinesFile));
        }

        [Fact]
        public void Add_AlreadyAdded_ReturnsFalse()
        {
            var engine = SearchEngineFactory.New();
            engine.Add(TestTextFiles.OneLineFile);
            Assert.False(engine.Add(TestTextFiles.OneLineFile));
        }
    }
}