using System.Collections.Generic;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.FileVersioning;

namespace Jbta.SearchEngine.FileIndexing
{
    internal interface IIndex
    {
        void Add(IFileVersion fileVersion, IEnumerable<ParsedWord> words);
        void Remove(IReadOnlyCollection<FileVersion> fileVersions);
        IEnumerable<WordEntry> Get(string query, bool wholeWord);
    }
}