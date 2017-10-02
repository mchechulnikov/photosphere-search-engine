using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jbta.SearchEngine.Events;
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

        public event FileIndexingEventHandler FileIndexingStarted;
        public event FileIndexingEventHandler FileIndexed;

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

            Task.Run(() => IndexFile(filePath));
        }

        private void IndexFile(string filePath)
        {
            FileIndexingStarted?.Invoke(new FileIndexingEventArgs(filePath));

            var fileVersion = _filesVersionsRegistry.RegisterFileVersion(filePath);

            var fileParser = _parserProvider.Provide(filePath);
            var words = fileParser.Parse(fileVersion);

            _index.Add(fileVersion, words);

            FileIndexed?.Invoke(new FileIndexingEventArgs(filePath));
        }
    }
}