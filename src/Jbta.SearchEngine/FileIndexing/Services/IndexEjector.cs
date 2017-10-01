using System;
using System.Linq;

namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal class IndexEjector : IIndexEjector
    {
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly IIndex _index;
        private readonly Settings _settings;

        public IndexEjector(
            FilesVersionsRegistry filesVersionsRegistry,
            IIndex index,
            Settings settings)
        {
            _filesVersionsRegistry = filesVersionsRegistry;
            _index = index;
            _settings = settings;
        }

        public void Eject(string path)
        {
            if (_filesVersionsRegistry.Contains(path))
            {
                RemoveFileFromIndex(path);
            }
            else
            {
                var filesPathes = _filesVersionsRegistry.Files.Where(p => p.StartsWith(path));
                foreach (var filePath in filesPathes)
                {
                    RemoveFileFromIndex(filePath);
                }
            }
        }

        private void RemoveFileFromIndex(string filePath)
        {
            var fileVersions = _filesVersionsRegistry.Get(filePath);
            foreach (var fileVersion in fileVersions)
            {
                RemoveFileVersionFromIndex(fileVersion);
            }
            _filesVersionsRegistry.Remove(filePath);
        }

        private void RemoveFileVersionFromIndex(FileVersion fileVersion)
        {
            _index.Remove(fileVersion);

            if (_settings.GcCollect)
            {
                GC.Collect();
            }
        }
    }
}