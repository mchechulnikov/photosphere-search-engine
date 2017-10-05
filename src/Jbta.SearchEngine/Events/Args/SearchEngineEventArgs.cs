using System;

namespace Jbta.SearchEngine.Events.Args
{
    public class SearchEngineEventArgs : EventArgs
    {
        public SearchEngineEventArgs(string filePath, Exception exception = null)
        {
            FilePath = filePath;
            Error = exception;
        }

        public SearchEngineEventArgs(Exception error)
        {
            Error = error;
        }

        public string FilePath { get; }

        public Exception Error { get; }
    }
}