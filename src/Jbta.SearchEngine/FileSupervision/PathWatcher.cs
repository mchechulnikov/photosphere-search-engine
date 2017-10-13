using System;
using System.IO;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileSupervision
{
    internal class PathWatcher : IDisposable
    {
        private readonly FileSystemWatcher _mainWatchder;
        private readonly FileSystemWatcher _additionalWatcher;

        public PathWatcher(
            string path,
            Action<FileSystemWatcher> subscribeOnFileEvents,
            Action<FileSystemWatcher> subscribeOnParentDirectoryEvents)
        {
            if (FileSystem.IsDirectory(path))
            {
                _mainWatchder = NewDirectoryContentWatcher(path);
                subscribeOnFileEvents(_mainWatchder);

                _additionalWatcher = NewDirectoryWatcher(path);
                if (_additionalWatcher != null)
                {
                    subscribeOnParentDirectoryEvents(_additionalWatcher);
                }
            }
            else
            {
                _mainWatchder = NewFileWatcher(path);
                subscribeOnFileEvents(_mainWatchder);
            }
        }

        public void Enable() => ToggleEventRaising(true);

        public void Dispose()
        {
            ToggleEventRaising(false);

            _mainWatchder?.Dispose();
            _additionalWatcher?.Dispose();
        }

        private static FileSystemWatcher NewDirectoryContentWatcher(string path)
        {
            return new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                Path = path,
                IncludeSubdirectories = true
            };
        }

        private static FileSystemWatcher NewDirectoryWatcher(string path)
        {
            var parentDirectoryPath = FileSystem.GetParentDirectoryPathByDirectoryPath(path);
            if (parentDirectoryPath == null)
            {
                return null;
            }

            return new FileSystemWatcher
            {
                //NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.Attributes,
                Path = parentDirectoryPath,
                Filter = new DirectoryInfo(path).Name
            };
        }

        private static FileSystemWatcher NewFileWatcher(string path)
        {
            return new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName,
                Path = FileSystem.GetDirectoryPathByFilePath(path),
                Filter = new FileInfo(path).Name
            };
        }

        private void ToggleEventRaising(bool enable)
        {
            _mainWatchder.EnableRaisingEvents = enable;

            if (_additionalWatcher != null)
            {
                _additionalWatcher.EnableRaisingEvents = enable;
            }
        }
    }
}