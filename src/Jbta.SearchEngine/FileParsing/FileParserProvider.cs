using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jbta.SearchEngine.Utils;

namespace Jbta.SearchEngine.FileParsing
{
    internal class FileParserProvider
    {
        private readonly IFileParser _defaultParser;
        private readonly IEnumerable<IFileParser> _customParsers;

        public FileParserProvider(Settings settings)
        {
            _defaultParser = new StandartFileParser(settings);
            _customParsers = settings.FileParsers;
        }

        public IFileParser Provide(string filePath)
        {
            if (FileSystem.IsDirectory(filePath))
            {
                throw new ArgumentException("Expected path of file", nameof(filePath));
            }
            var extension = Path.GetExtension(filePath);
            if (_customParsers == null || !_customParsers.Any())
            {
                return _defaultParser;
            }
            return _customParsers.FirstOrDefault(p => p.FileExtensions.Contains(extension)) ?? _defaultParser;
        }
    }
}