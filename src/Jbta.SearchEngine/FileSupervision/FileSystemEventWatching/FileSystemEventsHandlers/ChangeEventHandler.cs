using Jbta.SearchEngine.FileIndexing;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileSupervision.FileSystemEventWatching.FileSystemEventsHandlers
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