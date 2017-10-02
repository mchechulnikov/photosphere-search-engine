using System;
using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.Events;

namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal class IndexEjector : IIndexEjector
    {
        private readonly IIndex _index;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly Settings _settings;

        public IndexEjector(
            IIndex index,
            FilesVersionsRegistry filesVersionsRegistry,
            Settings settings)
        {
            _index = index;
            _filesVersionsRegistry = filesVersionsRegistry;
            _settings = settings;
        }

        public event FileIndexingEventHandler FileRemovedFromIndex;

        public void Eject(string path)
        {
            if (_filesVersionsRegistry.Contains(path))
            {
                RemoveFileFromIndex(path);
            }
            else
            {
                var filesPathes = _filesVersionsRegistry.Files.Where(p => p.StartsWith(path)).ToList();
                foreach (var filePath in filesPathes)
                {
                    RemoveFileFromIndex(filePath);
                }
            }
        }

        private void RemoveFileFromIndex(string filePath)
        {
            Task.Run(() =>
            {
                var fileVersions = _filesVersionsRegistry.Get(filePath);
                foreach (var fileVersion in fileVersions)
                {
                    RemoveFileVersionFromIndex(fileVersion);
                }
                _filesVersionsRegistry.Remove(filePath);

                FileRemovedFromIndex?.Invoke(new FileIndexingEventArgs(filePath));
            });
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