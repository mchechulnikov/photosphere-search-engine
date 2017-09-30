namespace Jbta.SearchEngine.Index
{
    internal class SearchSettings
    {
        public SearchSettings(bool caseSensetive, bool wholeWord)
        {
            CaseSensetive = caseSensetive;
            WholeWord = wholeWord;
        }

        public bool CaseSensetive { get; }

        public bool WholeWord { get; }
    }
}