using System.Collections.Generic;

namespace Jbta.SearchEngine.FileParsing
{
    public interface IFileParser
    {
        IEnumerable<string> FileExtensions { get; }
        IEnumerable<ParsedWord> Parse(IFileVersion fileVersion);
    }
}