using System.IO;

namespace Jbta.SearchEngine.DemoApp.Utils
{
    internal static class FileSystem
    {
        public static bool IsDirectory(string path) =>
            File.GetAttributes(path).HasFlag(FileAttributes.Directory);
    }
}