namespace Jbta.SearchEngine.Events.Args
{
    public class FilePathChangedEventArgs : SearchEngineEventArgs
    {
        public FilePathChangedEventArgs(string oldFilePath, string filePath) : base(filePath) {}

        public string OldFilePath { get; }
    }
}