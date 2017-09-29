namespace Jbta.Indexing.Indexing
{
    internal class SearchSettings
    {
        public SearchSettings(bool caseSensetive, bool wholeWord)
        {
            CaseSensetive = caseSensetive;
            WholeWord = wholeWord;
        }

        public bool CaseSensetive { get; set; }

        public bool WholeWord { get; set; }
    }
}