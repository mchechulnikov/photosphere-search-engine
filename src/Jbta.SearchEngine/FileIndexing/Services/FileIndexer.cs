using System.IO;
using System.Linq;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileIndexing.Services
{
    internal class FileIndexer : IFileIndexer
    {
        private readonly FileParserProvider _parserProvider;
        private readonly IIndex _index;
        private readonly FilesVersionsRegistry _filesVersionsRegistry;
        private readonly Settings _settings;

        public FileIndexer(
            FileParserProvider parserProvider,
            IIndex index,
            FilesVersionsRegistry filesVersionsRegistry,
            Settings settings)
        {
            _parserProvider = parserProvider;
            _index = index;
            _filesVersionsRegistry = filesVersionsRegistry;
            _settings = settings;
        }

        public void Index(string path)
        {
            if (FileSystem.IsDirectory(path))
            {
                var filesPathes = FileSystem.GetFilesPathesByDirectory(path).ToArray();
                foreach (var filePath in filesPathes)
                {
                    LoadFile(filePath);
                }
            }
            else
            {
                LoadFile(path);
            }
        }

        private void LoadFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            if (_settings.SupportedFilesExtensions.Contains(extension))
            {
                return;
            }

            var fileVersion = _filesVersionsRegistry.RegisterFileVersion(filePath);

            var fileParser = _parserProvider.Provide(filePath);
            var words = fileParser.Parse(fileVersion);

            _index.Add(fileVersion, words);
        }
    }
}