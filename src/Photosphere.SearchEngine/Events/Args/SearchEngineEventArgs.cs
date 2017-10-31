using System;

namespace Photosphere.SearchEngine.Events.Args
{
    public class SearchEngineEventArgs : EventArgs
    {
        public SearchEngineEventArgs(string path, Exception exception = null)
        {
            Path = path;
            Error = exception;
        }

        public SearchEngineEventArgs(Exception error)
        {
            Error = error;
        }

        public string Path { get; }

        public Exception Error { get; }
    }
}