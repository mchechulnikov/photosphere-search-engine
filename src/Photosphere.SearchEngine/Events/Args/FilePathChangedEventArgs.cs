namespace Photosphere.SearchEngine.Events.Args
{
    public class FilePathChangedEventArgs : SearchEngineEventArgs
    {
        public FilePathChangedEventArgs(string oldFilePath, string path) : base(path)
        {
            OldFilePath = oldFilePath;
        }

        public string OldFilePath { get; }
    }
}