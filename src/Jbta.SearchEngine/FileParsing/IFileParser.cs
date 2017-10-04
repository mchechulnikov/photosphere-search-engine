using System.Collections.Generic;
using Jbta.SearchEngine.FileVersioning;

namespace Jbta.SearchEngine.FileParsing
{
    public interface IFileParser
    {
        IEnumerable<string> FileExtensions { get; }
        IEnumerable<ParsedWord> Parse(IFileVersion fileVersion);
    }
}