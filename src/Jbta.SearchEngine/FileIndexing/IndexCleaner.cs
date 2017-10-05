using System;
using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.FileVersioning;
using Jbta.SearchEngine.Index;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class IndexCleaner : ICleaner
    {
        private readonly IIndex _index;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly SearchEngineSettings _settings;

        public IndexCleaner(
            IIndex index,
            FilesVersionsRegistry filesVersionsRegistry,
            SearchEngineSettings settings)
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