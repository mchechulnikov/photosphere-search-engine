using System;

namespace Jbta.SearchEngine.Events
{
    public class FileEventArgs : EventArgs
    {
        public FileEventArgs(string filePath, string fileName)
        {
            FilePath = filePath;
            FileName = fileName;
        }

        public string FilePath { get; }

        public string FileName { get; }
    }
}