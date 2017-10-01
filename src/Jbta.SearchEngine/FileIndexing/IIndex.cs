using System.Collections.Generic;
using Jbta.SearchEngine.FileParsing;

namespace Jbta.SearchEngine.FileIndexing
{
    internal interface IIndex
    {
        void Add(FileVersion fileVersion, IEnumerable<ParsedWord> words);
        void Remove(FileVersion fileVersion);
        void Remove(IReadOnlyCollection<FileVersion> fileVersions);
        IEnumerable<WordEntry> Get(string query, bool wholeWord);
    }
}