namespace Photosphere.SearchEngine.Vendor.VsCodeFilewatcher
{
    internal class FileSystemEvent
    {
        public ChangeType ChangeType { get; set; }
        public string OldPath { get; set; }
        public string Path { get; set; }
    }
}