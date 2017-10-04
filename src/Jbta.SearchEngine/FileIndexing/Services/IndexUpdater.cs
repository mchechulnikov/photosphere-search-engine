using System.Linq;
using Jbta.SearchEngine.FileVersioning;

namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal class IndexUpdater : IIndexUpdater
    {
        private readonly IFileIndexer _fileIndexer;
        private readonly IIndex _index;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;

        public IndexUpdater(
            IFileIndexer fileIndexer,
            IIndex index,
            FilesVersionsRegistry filesVersionsRegistry)
        {
            _fileIndexer = fileIndexer;
            _index = index;
            _filesVersionsRegistry = filesVersionsRegistry;
        }

        public void Update(string filePath)
        {
            if (!_filesVersionsRegistry.IsNeedToBeUpdated(filePath))
            {
                return;
            }

            _fileIndexer.Index(filePath);
            RemoveIrrelevantFileVersions(filePath);
        }

        private void RemoveIrrelevantFileVersions(string filePath)
        {
            var irrelevantFileVersions = _filesVersionsRegistry.RemoveIrrelevantFileVersions(filePath);
            if (!irrelevantFileVersions.Any())
            {
                return;
            }

            _index.Remove(irrelevantFileVersions);
        }
    }
}