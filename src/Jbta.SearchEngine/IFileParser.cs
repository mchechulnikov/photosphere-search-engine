using System.Collections.Generic;

namespace Jbta.SearchEngine
{
    public interface IFileParser
    {
        IEnumerable<string> FileExtensions { get; }

        IEnumerable<(string word, WordEntry)> Parse(IFileVersion fileVersion);
    }
}