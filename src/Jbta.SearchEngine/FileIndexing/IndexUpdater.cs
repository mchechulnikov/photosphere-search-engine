using System.IO;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class IndexUpdater : IIndexUpdater
    {
        private readonly IFileIndexer _fileIndexer;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;

        public IndexUpdater(
            IFileIndexer fileIndexer,
            FilesVersionsRegistry filesVersionsRegistry)
        {
            _fileIndexer = fileIndexer;
            _filesVersionsRegistry = filesVersionsRegistry;
        }

        public void Update(string filePath)
        {
            if (!FileSystem.IsExistingPath(filePath))
            {
                return;
            }
            if (FileSystem.IsDirectory(filePath))
            {
                return;
            }
            if (!_filesVersionsRegistry.IsFileUpdatable(filePath))
            {
                return;
            };

            var irrelevantVersions = _filesVersionsRegistry.Get(filePath);

            if (File.Exists(filePath))
            {
                _fileIndexer.Index(filePath);
            }

            if (irrelevantVersions != null)
            {
                _filesVersionsRegistry.KillVersions(irrelevantVersions);
            }
        }
    }
}