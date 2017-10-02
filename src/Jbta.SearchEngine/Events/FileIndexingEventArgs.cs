using System;

namespace Jbta.SearchEngine.Events
{
    public class FileIndexingEventArgs : EventArgs
    {
        public FileIndexingEventArgs(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}