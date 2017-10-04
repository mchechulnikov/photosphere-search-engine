using System.IO;
using Jbta.SearchEngine.FileIndexing.Services;
using Jbta.SearchEngine.FileVersioning.Services;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class FileSystemWatcherFactory
    {
        private const NotifyFilters WatcherNotifyFilters =
            NotifyFilters.LastWrite
            | NotifyFilters.FileName
            | NotifyFilters.DirectoryName;

        private readonly IFileIndexer _indexer;
        private readonly IIndexUpdater _indexUpdater;
        private readonly IIndexEjector _indexEjector;
        private readonly IFilePathActualizer _filePathActualizer;

        public FileSystemWatcherFactory(
            IFileIndexer indexer,
            IIndexUpdater indexUpdater,
            IIndexEjector indexEjector,
            IFilePathActualizer filePathActualizer)
        {
            _indexer = indexer;
            _indexUpdater = indexUpdater;
            _indexEjector = indexEjector;
            _filePathActualizer = filePathActualizer;
        }

        public FileSystemWatcher New(string path)
        {
            // TODO check path for already added
            var watcher = new FileSystemWatcher { NotifyFilter = WatcherNotifyFilters };
            if (FileSystem.IsDirectory(path))
            {
                watcher.Path = path;
                watcher.IncludeSubdirectories = true;
            }
            else
            {
                watcher.Path = FileSystem.GetDirectoryPathByFilePath(path);
                watcher.Filter = new FileInfo(path).Name;
            }
            SubscribeOnFileEvents(watcher);
            return watcher;
        }

        private void SubscribeOnFileEvents(FileSystemWatcher watcher)
        {
            watcher.Created += (o, e) => _indexer.Index(e.FullPath);
            watcher.Changed += (o, e) => _indexUpdater.Update(e.FullPath);
            watcher.Deleted += (o, e) => _indexEjector.Eject(e.FullPath);
            watcher.Renamed += (o, e) => _filePathActualizer.Actualize(e.OldFullPath, e.FullPath);
        }
    }
}