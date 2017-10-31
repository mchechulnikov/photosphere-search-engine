using System;
using Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers;
using Photosphere.SearchEngine.Vendor.VsCodeFilewatcher;

namespace Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching
{
    internal class FileSystemEventHandler
    {
        private readonly CreateEventHandler _createEventHandler;
        private readonly ChangeEventHandler _changeEventHandler;
        private readonly DeleteEventHandler _deleteEventHandler;
        private readonly RenameEventHandler _renameEventHandler;

        public FileSystemEventHandler(
            CreateEventHandler createEventHandler,
            ChangeEventHandler changeEventHandler,
            DeleteEventHandler deleteEventHandler,
            RenameEventHandler renameEventHandler)
        {
            _createEventHandler = createEventHandler;
            _changeEventHandler = changeEventHandler;
            _deleteEventHandler = deleteEventHandler;
            _renameEventHandler = renameEventHandler;
        }

        public void OnEvent(FileSystemEvent e)
        {
            var path = e.Path;
            switch (e.ChangeType)
            {
                case ChangeType.Created:
                    _createEventHandler.Handle(path);
                    break;
                case ChangeType.Changed:
                    _changeEventHandler.Handle(path);
                    break;
                case ChangeType.Deleted:
                    _deleteEventHandler.Handle(path);
                    break;
                case ChangeType.Rename:
                    _renameEventHandler.Handle(e.OldPath, path);
                    break;
                case ChangeType.Log:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}