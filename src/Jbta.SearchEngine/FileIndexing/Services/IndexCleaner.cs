using System;
using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.FileVersioning;

namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal class IndexCleaner : ICleaner
    {
        private readonly IIndex _index;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly Settings _settings;

        public IndexCleaner(
            IIndex index,
            FilesVersionsRegistry filesVersionsRegistry,
            Settings settings)
        {
            _index = index;
            _filesVersionsRegistry = filesVersionsRegistry;
            _settings = settings;
        }

        public bool IsBusy { get; private set; }

        public void CleanUp()
        {
            Task.Run(() =>
            {
                IsBusy = true;

                var deadVersions = _filesVersionsRegistry.RemoveDeadVersions();
                if (!deadVersions.Any())
                {
                    IsBusy = false;
                    return;
                }

                _index.Remove(deadVersions);
                if (_settings.GcCollect)
                {
                    GC.Collect();
                }

                IsBusy = false;
            });
        }
    }
}