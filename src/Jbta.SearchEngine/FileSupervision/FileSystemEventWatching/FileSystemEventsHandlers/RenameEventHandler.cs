using Jbta.SearchEngine.FileVersioning.Services;

namespace Jbta.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers
{
    internal class RenameEventHandler
    {
        private readonly IFilePathActualizer _filePathActualizer;

        public RenameEventHandler(IFilePathActualizer filePathActualizer)
        {
            _filePathActualizer = filePathActualizer;
        }

        public void Handle(string oldPath, string newPath)
        {
            _filePathActualizer.Actualize(oldPath, newPath);
        }
    }
}