using System;
using System.IO;
using System.Threading;

namespace Jbta.SearchEngine.IntegrationTests.Utils
{
    internal class TestFile : IDisposable
    {
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
            if (!File.Exists(Path))
            {
                return;
            }

            File.Delete(Path);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(100);
        }
    }
}