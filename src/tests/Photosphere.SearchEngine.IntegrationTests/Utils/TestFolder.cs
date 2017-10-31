using System;
using System.IO;

namespace Photosphere.SearchEngine.IntegrationTests.Utils
{
    internal class TestFolder : IDisposable
    {
        private bool _isDeleted;

        public TestFolder(string folder = ".")
        {
            Path = $"{folder}\\test-{Guid.NewGuid()}";
            Directory.CreateDirectory(Path);
        }

        public string Path { get; private set; }

        public DateTime LastWriteTimeUtc => new DirectoryInfo(Path).LastWriteTimeUtc;

        public void Rename(string newName)
        {
            var newPath = GetNewPath(newName);
            Directory.Move(Path, newPath);
            Path = newPath;
        }

        public void Dispose()
        {
            Delete();
        }

        public void Delete()
        {
            if (_isDeleted)
            {
                return;
            }
            if (!Directory.Exists(Path))
            {
                return;
            }

            while (!_isDeleted)
            {
                try
                {
                    Directory.Delete(Path, true);
                    _isDeleted = true;
                }
                catch
                {
                    // ignored
                }
            }
        }

        private string GetNewPath(string newName)
        {
            var newPath = new DirectoryInfo(Path).Parent?.FullName;
            if (newPath == null)
            {
                throw new InvalidOperationException("Path can not be null");
            }
            newPath += newPath.EndsWith("\\") ? newName : "\\" + newName;
            return newPath;
        }
    }
}