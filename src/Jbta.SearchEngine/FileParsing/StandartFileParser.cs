using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Jbta.SearchEngine.FileParsing
{
    internal class StandartFileParser : IFileParser
    {
        private readonly SearchEngineSettings _settings;

        public StandartFileParser(SearchEngineSettings settings)
        {
            _settings = settings;
        }

        public IEnumerable<string> FileExtensions => _settings.SupportedFilesExtensions;

        public IEnumerable<ParsedWord> Parse(IFileVersion fileVersion, Encoding encoding = null)
        {
            using (var reader = GetStreamReader(fileVersion, encoding))
            {
                foreach (var parsedWord in Parse(fileVersion, reader))
                {
                    yield return parsedWord;
                }
            }
        }

        private static StreamReader GetStreamReader(IFileVersion fileVersion, Encoding encoding)
        {
            return encoding == null
                ? new StreamReader(fileVersion.Path)
                : new StreamReader(fileVersion.Path, encoding);
        }

        private static IEnumerable<ParsedWord> Parse(IFileVersion fileVersion, TextReader reader)
        {
            const int bufferSize = 2048;
            var buffer = new char[bufferSize];
            var word = new StringBuilder();
            var position = 1;
            var lineNumber = 1;
            while (reader.ReadBlock(buffer, 0, bufferSize) != 0)
            {
                foreach (var character in buffer)
                {
                    position++;
                    if (char.IsWhiteSpace(character))
                    {
                        if (character == '\n')
                        {
                            lineNumber++;
                            position = 1;
                        }
                        if (word.Length < 1)
                        {
                            continue;
                        }

                        var wordString = word.ToString();
                        yield return new ParsedWord(
                            wordString,
                            new WordEntry(fileVersion, position - wordString.Length - 1, lineNumber)
                        );
                        word.Clear();
                    }
                    else if (character == '\0')
                    {
                        var wordString = word.ToString();
                        if (string.IsNullOrWhiteSpace(wordString))
                        {
                            break;
                        }

                        yield return new ParsedWord(
                            wordString,
                            new WordEntry(fileVersion, position - wordString.Length - 1, lineNumber)
                        );
                        word.Clear();
                        break;
                    }
                    else if (!char.IsPunctuation(character))
                    {
                        word.Append(character);
                    }
                }
            }
        }
    }
}