using Photosphere.SearchEngine.FileIndexing;
using Photosphere.SearchEngine.Utils;

namespace Photosphere.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers
{
    internal class ChangeEventHandler
     {
         private readonly IIndexUpdater _indexUpdater;

         public ChangeEventHandler(IIndexUpdater indexUpdater)
         {
             _indexUpdater = indexUpdater;
         }

        public void Handle(string path)
        {
            if (!FileSystem.IsExistingPath(path))
            {
                return;
            }
            if (FileSystem.IsDirectory(path))
            {
                return;
            }

            _indexUpdater.Update(path);
        }
    }
}