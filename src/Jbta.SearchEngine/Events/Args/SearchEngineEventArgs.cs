using System;

namespace Jbta.SearchEngine.Events.Args
{
    public class SearchEngineEventArgs : EventArgs
    {
        public SearchEngineEventArgs(string filePath)
        {
            FilePath = filePath;
        }

        public string FilePath { get; }
    }
}