namespace Jbta.SearchEngine.Events.Args
{
    public class FilePathChangedEventArgs : SearchEngineEventArgs
    {
        public FilePathChangedEventArgs(string oldFilePath, string filePath) : base(filePath)
        {
            OldFilePath = oldFilePath;
        }

        public string OldFilePath { get; }
    }
}