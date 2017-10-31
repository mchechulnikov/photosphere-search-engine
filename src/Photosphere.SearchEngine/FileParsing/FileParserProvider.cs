using System;
using System.IO;
using System.Linq;
using Photosphere.SearchEngine.Utils;

namespace Photosphere.SearchEngine.FileParsing
{
    internal class FileParserProvider
    {
        private readonly SearchEngineSettings _settings;
        private readonly IFileParser _defaultParser;

        public FileParserProvider(SearchEngineSettings settings)
        {
            _settings = settings;
            _defaultParser = new StandartFileParser(settings);
        }

        public IFileParser Provide(string filePath)
        {
            if (FileSystem.IsDirectory(filePath))
            {
                throw new ArgumentException("Expected path of file, not path of directory", nameof(filePath));
            }
            var extension = Path.GetExtension(filePath);
            if (_settings.FileParsers == null || !_settings.FileParsers.Any())
            {
                return _defaultParser;
            }
            return _settings.FileParsers.FirstOrDefault(p => p.FileExtensions.Contains(extension)) ?? _defaultParser;
        }
    }
}