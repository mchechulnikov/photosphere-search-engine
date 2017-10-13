using System;
using System.IO;

namespace Jbta.SearchEngine.IntegrationTests.Utils
{
    internal class TestFile : IDisposable
    {
        private bool _isDeleted;

        public TestFile(string content, string folderPath = ".", string fileName = null)
        {
            fileName = fileName ?? GenerateFileName();
            Path = $"{folderPath}\\{fileName}";
            using (var file = new StreamWriter(Path, true))
            {
                file.WriteLine(content);
            }
            File.SetAttributes(Path, FileAttributes.Normal);
        }

        public static string GenerateFileName()
        {
            return $"test-{Guid.NewGuid()}.txt";
        }

        public string Name => new FileInfo(Path).Name;

        public string Path { get; private set; }

        public void ChangeContent(string newContent)
        {
            var text = File.ReadAllText(Path);
            text = text.Replace(text, newContent);
            File.WriteAllText(Path, text);
        }

        public void Rename(string newName)
        {
            var newPath = new FileInfo(Path).DirectoryName;
            newPath += newPath.EndsWith("\\") ? newName : "\\" + newName;
            File.Move(Path, newPath);
            Path = newPath;
        }

        public void Move(string folderPath)
        {
            folderPath = folderPath.Replace(".\\", string.Empty);
            var directoryPath = new FileInfo(Path).Directory.Name;
            var newFilePath = Path.Replace(directoryPath, folderPath);
            File.Move(Path, newFilePath);
            Path = newFilePath;
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
            if (!File.Exists(Path))
            {
                return;
            }
            while (!_isDeleted)
            {
                try
                {
                    File.Delete(Path);
                    _isDeleted = true;
                }
                catch
                {
                    // ignored
                }
            }
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}