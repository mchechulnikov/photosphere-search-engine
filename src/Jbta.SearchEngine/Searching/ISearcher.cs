using System.Collections.Generic;

namespace Jbta.SearchEngine.Searching
{
    internal interface ISearcher
    {
        IEnumerable<WordEntry> Search(string query, bool wholeWord = false);
        IEnumerable<string> SearchFiles(string query, bool wholeWord = false);
    }
}