using System.Collections.Generic;
using System.IO;

namespace Jbta.SearchEngine.Utils
{
    internal static class FileSystem
    {
        public static IEnumerable<string> GetFilesPathesByDirectory(string directoryPath)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            foreach (var file in directoryInfo.EnumerateFiles())
            {
                yield return string.Intern(file.FullName);
            }

            foreach (var subdirectory in directoryInfo.EnumerateDirectories())
            {
                foreach (var filePath in GetFilesPathesByDirectory(subdirectory.FullName))
                {
                    yield return filePath;
                }
            }
        }
    }
}