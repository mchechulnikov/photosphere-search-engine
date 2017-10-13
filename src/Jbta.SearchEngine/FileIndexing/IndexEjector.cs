using System;
using System.Threading.Tasks;
using Jbta.SearchEngine.Events;
using Jbta.SearchEngine.FileVersioning;

namespace Jbta.SearchEngine.FileIndexing
{
    internal class IndexEjector : IIndexEjector
    {
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly IEventReactor _eventReactor;

        public IndexEjector(
            IEventReactor eventReactor,
            FilesVersionsRegistry filesVersionsRegistry)
        {
            _eventReactor = eventReactor;
            _filesVersionsRegistry = filesVersionsRegistry;
        }

        public void Eject(string path)
        {
            if (_filesVersionsRegistry.Contains(path))
            {
                EjectFileFromIndex(path);
            }
            else
            {
                EjectDirectory(path);
            }
        }

        public void EjectFile(string filePath)
        {
            if (_filesVersionsRegistry.Contains(filePath))
            {
                EjectFileFromIndex(filePath);
            }
        }

        private void EjectFileFromIndex(string filePath)
        {
            Task.Run(() =>
            {
                try
                {
                    _eventReactor.React(EngineEvent.FileRemovingStarted, filePath);
                    _filesVersionsRegistry.KillAllVersions(filePath);
                    _eventReactor.React(EngineEvent.FileRemovingEnded, filePath);
                }
                catch (Exception exception)
                {
                    _eventReactor.React(EngineEvent.FileRemovingEnded, filePath, exception);
                }
            });
        }

        private void EjectDirectory(string directoryPath)
        {
            var filesPathes = _filesVersionsRegistry.GetAliveFiles(directoryPath);
            foreach (var filePath in filesPathes)
            {
                EjectFileFromIndex(filePath);
            }
        }
    }
}