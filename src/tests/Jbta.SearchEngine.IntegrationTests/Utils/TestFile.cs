using System;
using System.IO;

namespace Jbta.SearchEngine.IntegrationTests.Utils
{
    internal class TestFile : IDisposable
    {
        public TestFile(string filePath, string content)
        {
            Path = filePath;
            using (var file = new StreamWriter(filePath, true))
            {
                file.WriteLine(content);
            }
        }

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

        public void Dispose()
        {
            File.Delete(Path);
        }
    }
}