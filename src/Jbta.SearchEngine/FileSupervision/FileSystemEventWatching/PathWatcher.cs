using System;
using System.IO;
using Jbta.SearchEngine.Utils;
using Jbta.SearchEngine.Vendor.VsCodeFilewatcher;

namespace Jbta.SearchEngine.FileSupervision.FileSystemEventWatching
{
    internal class PathWatcher : IDisposable
    {
        private const int FileSystemWatcherBufferSize = 32768;
        private readonly Action<FileSystemEvent> _eventCallback;
        private FileSystemWatcher _mainWatcher;
        private FileSystemWatcher _additionalWatcher;

        public PathWatcher(string path, Action<FileSystemEvent> eventCallback)
        {
            _eventCallback = eventCallback;
            Initialize(path);
        }

        public void Enable() => ToggleEventRaising(true);

        public void Reset(string path)
        {
            var oldMainWatcher = _mainWatcher;
            var oldAddtionalWatcher = _additionalWatcher;

            Initialize(path);
            Enable();

            UtilizeWatcher(oldMainWatcher);
            UtilizeWatcher(oldAddtionalWatcher);
        }

        public void Dispose()
        {
            ToggleEventRaising(false);
            _mainWatcher?.Dispose();
            _additionalWatcher?.Dispose();
        }

        private void Initialize(string path)
        {
            if (FileSystem.IsDirectory(path))
            {
                _mainWatcher = NewDirectoryContentWatcher(path);
                _additionalWatcher = NewDirectoryWatcher(path);
            }
            else
            {
                _mainWatcher = NewFileWatcher(path);
            }
        }

        private FileSystemWatcher NewDirectoryContentWatcher(string path)
        {
            var watcher = new FileSystemWatcher
            {
                InternalBufferSize = FileSystemWatcherBufferSize,
                Path = path,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                IncludeSubdirectories = true
            };
            watcher.Changed += (source, e) => { ProcessEvent(e, ChangeType.Changed); };
            watcher.Created += (source, e) => { ProcessEvent(e, ChangeType.Created); };
            watcher.Deleted += (source, e) => { ProcessEvent(e, ChangeType.Deleted); };
            watcher.Renamed += (source, e) => { ProcessRenameEvent(e); };
            return watcher;
        }

        private FileSystemWatcher NewDirectoryWatcher(string path)
        {
            var parentDirectoryPath = FileSystem.GetParentDirectoryPathByDirectoryPath(path);
            if (parentDirectoryPath == null)
            {
                return null;
            }

            var watcher = new FileSystemWatcher
            {
                InternalBufferSize = FileSystemWatcherBufferSize,
                Path = parentDirectoryPath,
                Filter = new DirectoryInfo(path).Name,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };
            watcher.Deleted += (source, e) => { ProcessEvent(e, ChangeType.Deleted); };
            watcher.Renamed += (source, e) => { ProcessRenameEvent(e); };
            return watcher;
        }

        private FileSystemWatcher NewFileWatcher(string path)
        {
            var watcher = new FileSystemWatcher
            {
                InternalBufferSize = FileSystemWatcherBufferSize,
                Path = FileSystem.GetDirectoryPathByFilePath(path),
                Filter = new FileInfo(path).Name,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
            };
            watcher.Changed += (source, e) => { ProcessEvent(e, ChangeType.Changed); };
            watcher.Created += (source, e) => { ProcessEvent(e, ChangeType.Created); };
            watcher.Deleted += (source, e) => { ProcessEvent(e, ChangeType.Deleted); };
            watcher.Renamed += (source, e) => { ProcessRenameEvent(e); };
            return watcher;
        }

        private void ProcessEvent(FileSystemEventArgs e, ChangeType changeType)
        {
            _eventCallback(new FileSystemEvent
            {
                ChangeType = changeType,
                Path = e.FullPath
            });
        }

        private void ProcessRenameEvent(RenamedEventArgs e)
        {
            _eventCallback(new FileSystemEvent
            {
                ChangeType = ChangeType.Rename,
                OldPath = e.OldFullPath,
                Path = e.FullPath
            });
        }

        private void ToggleEventRaising(bool enable)
        {
            _mainWatcher.EnableRaisingEvents = enable;

            if (_additionalWatcher != null)
            {
                _additionalWatcher.EnableRaisingEvents = enable;
            }
        }

        private void UtilizeWatcher(FileSystemWatcher watcher)
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
            }
            watcher?.Dispose();
        }
    }
}