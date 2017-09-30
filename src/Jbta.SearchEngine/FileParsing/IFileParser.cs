using System.Collections.Generic;

namespace Jbta.SearchEngine.FileParsing
{
    public interface IFileParser
    {
        IEnumerable<(string word, WordEntry)> Parse(string filePath);
    }
}