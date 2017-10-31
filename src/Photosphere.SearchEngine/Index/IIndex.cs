using System.Collections.Generic;
using Photosphere.SearchEngine.FileParsing;
using Photosphere.SearchEngine.FileVersioning;

namespace Photosphere.SearchEngine.Index
{
    internal interface IIndex
    {
        void Add(IFileVersion fileVersion, IEnumerable<ParsedWord> words);
        void Remove(IReadOnlyCollection<FileVersion> fileVersions);
        IEnumerable<WordEntry> Get(string query, bool wholeWord);
    }
}