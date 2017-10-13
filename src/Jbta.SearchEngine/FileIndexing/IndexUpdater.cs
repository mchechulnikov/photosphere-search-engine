using System;
using System.IO;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileVersioning;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class IndexUpdater : IIndexUpdater
    {
        private readonly IEventReactor _eventReactor;
        private readonly IFileIndexer _fileIndexer;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;

        public IndexUpdater(
            IEventReactor eventReactor,
            IFileIndexer fileIndexer,
            FilesVersionsRegistry filesVersionsRegistry)
        {
            _eventReactor = eventReactor;
            _fileIndexer = fileIndexer;
            _filesVersionsRegistry = filesVersionsRegistry;
        }

        public void Update(string filePath)
        {
            try
            {
                if (!_filesVersionsRegistry.IsFileUpdatable(filePath))
                {
                    return;
                }

                var irrelevantVersions = _filesVersionsRegistry.Get(filePath);

                if (File.Exists(filePath))
                {
                    _eventReactor.React(EngineEvent.FileUpdateInitiated, filePath);
                    _fileIndexer.Index(filePath);
                }

                if (irrelevantVersions != null)
                {
                    _filesVersionsRegistry.KillVersions(irrelevantVersions);
                }
            }
            catch (Exception exception)
            {
                _eventReactor.React(EngineEvent.FileUpdateFailed, filePath, exception);
            }
        }
    }
}