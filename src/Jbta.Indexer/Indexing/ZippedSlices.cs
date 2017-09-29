namespace Jbta.Indexing.Indexing
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

        public MatchKind MatchKind =>
            FirstTail.Length == 0
                ? (SecondTail.Length == 0
                    ? MatchKind.Match
                    : MatchKind.IsContained)
                : (SecondTail.Length == 0
                    ? MatchKind.Contains
                    : MatchKind.Partial);
    }
}