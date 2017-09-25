using System.IO;

namespace Jbta.DemoApp.Utils
{
    public static class FileSystemUtils
    {
        public static string GetFolderName(string path) => Path.GetFileName(Path.GetDirectoryName(path));
    }
}