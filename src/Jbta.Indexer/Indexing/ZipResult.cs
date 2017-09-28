namespace Jbta.Indexing.Indexing
{
    public struct ZipResult
    {
        public ZipResult(StringSlice commonHead, StringSlice thisRest, StringSlice otherRest)
        {
            CommonHead = commonHead;
            ThisRest = thisRest;
            OtherRest = otherRest;
        }

        public MatchKind MatchKind
        {
            get
            {
                return ThisRest.Length == 0
                    ? (OtherRest.Length == 0
                        ? MatchKind.ExactMatch
                        : MatchKind.IsContained)
                    : (OtherRest.Length == 0
                        ? MatchKind.Contains
                        : MatchKind.Partial);
            }
        }

        public StringSlice OtherRest { get; }

        public StringSlice ThisRest { get; }

        public StringSlice CommonHead { get; }


        public bool Equals(ZipResult other)
        {
            return
                CommonHead == other.CommonHead &&
                OtherRest == other.OtherRest &&
                ThisRest == other.ThisRest;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ZipResult && Equals((ZipResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = CommonHead.GetHashCode();
                hashCode = (hashCode * 397) ^ OtherRest.GetHashCode();
                hashCode = (hashCode * 397) ^ ThisRest.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ZipResult left, ZipResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ZipResult left, ZipResult right)
        {
            return !(left == right);
        }
    }
}