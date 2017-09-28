namespace Jbta.Indexing.Indexing
{
    public struct SplitResult
    {
        public SplitResult(StringSlice head, StringSlice rest)
        {
            Head = head;
            Rest = rest;
        }

        public StringSlice Rest { get; }

        public StringSlice Head { get; }

        public bool Equals(SplitResult other)
        {
            return Head == other.Head && Rest == other.Rest;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is SplitResult && Equals((SplitResult)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Head.GetHashCode() * 397) ^ Rest.GetHashCode();
            }
        }

        public static bool operator ==(SplitResult left, SplitResult right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SplitResult left, SplitResult right)
        {
            return !(left == right);
        }
    }
}