using System.Linq;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileVersioning.Services
{
    internal class FilePathActualizer : IFilePathActualizer
    {
        private readonly FilesVersionsRegistry _registry;

        public FilePathActualizer(FilesVersionsRegistry registry)
        {
            _registry = registry;
        }

        public void Actualize(string oldPath, string newPath)
        {
            if (FileSystem.IsDirectory(newPath))
            {
                var oldFilesPathes = _registry.Files.Where(p => p.StartsWith(oldPath)).ToList();
                foreach (var oldFilePath in oldFilesPathes)
                {
                    var newFilePath = newPath + oldFilePath.Substring(oldPath.Length);
                    _registry.ChangeFilePath(oldFilePath, newFilePath);
                }
            }
            else
            {
                _registry.ChangeFilePath(oldPath, newPath);
            }
        }
    }
}