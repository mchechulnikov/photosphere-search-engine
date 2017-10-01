using System.Collections.Generic;
using Jbta.SearchEngine.FileIndexing;

namespace Jbta.SearchEngine.FileParsing
{
    public interface IFileParser
    {
        IEnumerable<string> FileExtensions { get; }

        IEnumerable<ParsedWord> Parse(IFileVersion fileVersion);
    }
}