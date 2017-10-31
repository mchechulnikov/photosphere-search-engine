using System;
using System.IO;
using System.Linq;
using Photosphere.SearchEngine.FileParsing;
using Photosphere.SearchEngine.FileVersioning;
using Photosphere.SearchEngine.IntegrationTests.Resources;
using Xunit;

namespace Photosphere.SearchEngine.IntegrationTests.FileParsing
{
    public class StandartFileParserTests
    {
        [Fact]
        public void Parse_OneLineFile_ExpectedParsedWordsCount()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.OneLineFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion);

            Assert.Equal(TestTextFiles.OneLineFileWordsCount, parsedWords.Count());
        }

        [Fact]
        public void Parse_OneLineFile_ExpectedWords()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.OneLineFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion);

            var expectedWords = OneLineFileContent.Split();
            foreach (var parsedWord in parsedWords)
            {
                Assert.True(expectedWords.Contains(parsedWord.Word));
            }
        }

        [Fact]
        public void Parse_OneLineFile_ExpectedLineNumbers()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.OneLineFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion);

            foreach (var wordEntries in parsedWords.Select(w => w.Entry))
            {
                Assert.Equal(1, wordEntries.LineNumber);
            }
        }

        [Fact]
        public void Parse_OneLineFile_ExpectedPositions()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.OneLineFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion);

            var expectedWords = OneLineFileContent.Split(' ').Select(w => new
            {
                Word = w,
                Position = OneLineFileContent.IndexOf(w, StringComparison.Ordinal) + 1
            }).ToList();
            foreach (var parsedWord in parsedWords)
            {
                var expectedPosition = expectedWords.FirstOrDefault(w => w.Word.Equals(parsedWord.Word))?.Position;
                Assert.NotNull(expectedPosition);
                Assert.Equal(expectedPosition, parsedWord.Entry.Position);
            }
        }

        [Fact]
        public void Parse_TwoLinesFile_ExpectedParsedWordsCount()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.TwoLinesFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion);

            Assert.Equal(TestTextFiles.TwoLinesFileWordsCount, parsedWords.Count());
        }

        [Fact]
        public void Parse_TwoLinesFile_ExpectedWords()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.TwoLinesFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion);

            var expectedWords = TwoLinesFileContent
                .Split()
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            foreach (var parsedWord in parsedWords)
            {
                Assert.True(expectedWords.Contains(parsedWord.Word));
            }
        }

        [Fact]
        public void Parse_TwoLinesFile_ExpectedLineNumbers()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.TwoLinesFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion).ToList();

            var expectedWords = TwoLinesFileContent.Split('\n');
            Assert.Equal(2, parsedWords.GroupBy(w => w.Entry.LineNumber).Count());
            foreach (var parsedWord in parsedWords.Where(w => w.Entry.LineNumber == 1))
            {
                Assert.True(expectedWords[0].Contains(parsedWord.Word));
            }
            foreach (var parsedWord in parsedWords.Where(w => w.Entry.LineNumber == 2))
            {
                Assert.True(expectedWords[1].Contains(parsedWord.Word));
            }
        }

        [Fact]
        public void Parse_TwoLinesFile_ExpectedPositions()
        {
            var parser = GetNewParser();
            var fileVersion = new FileVersion(TestTextFiles.TwoLinesFile, DateTime.UtcNow, DateTime.UtcNow);

            var parsedWords = parser.Parse(fileVersion).ToList();

            var lines = TwoLinesFileContentByLines;

            Check(lines[0], 1);
            Check(lines[1], 2);

            void Check(string line, int lineNumber)
            {
                var expectedWords = line.Split().Select(w => new
                {
                    Word = w,
                    Position = line.IndexOf(w, StringComparison.Ordinal) + 1
                }).ToList();
                foreach (var parsedWord in parsedWords.Where(w => w.Entry.LineNumber == lineNumber))
                {
                    var expectedPosition = expectedWords.FirstOrDefault(w => w.Word.Equals(parsedWord.Word))?.Position;
                    Assert.Equal(expectedPosition, parsedWord.Entry.Position);
                }
            }
        }

        private static string OneLineFileContent => File.ReadAllText(TestTextFiles.OneLineFile);
        private static string TwoLinesFileContent => File.ReadAllText(TestTextFiles.TwoLinesFile);
        private static string[] TwoLinesFileContentByLines => File.ReadAllLines(TestTextFiles.TwoLinesFile);

        private static IFileParser GetNewParser() => new StandartFileParser(new SearchEngineSettings());
    }
}