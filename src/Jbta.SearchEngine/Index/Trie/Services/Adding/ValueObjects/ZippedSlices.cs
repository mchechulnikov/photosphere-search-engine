using Jbta.SearchEngine.Index.Trie.ValueObjects;

namespace Jbta.SearchEngine.Index.Trie.Services.Adding.ValueObjects
{
    internal struct ZippedSlices
    {
        public ZippedSlices(StringSlice head, StringSlice firstTail, StringSlice secondTail)
        {
            Head = head;
            FirstTail = firstTail;
            SecondTail = secondTail;
        }

        public StringSlice Head { get; }

        public StringSlice SecondTail { get; }

        public StringSlice FirstTail { get; }

        public Match MatchKind =>
            FirstTail.Length == 0
                ? (SecondTail.Length == 0
                    ? Match.Match
                    : Match.IsContained)
                : (SecondTail.Length == 0
                    ? Match.Contains
                    : Match.Partial);

        public enum Match
        {
            Match,
            Contains,
            IsContained,
            Partial,
        }
    }
}