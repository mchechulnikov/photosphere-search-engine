namespace Jbta.Indexing.Indexing.Services
{
    internal class KeysZipper
    {
        public ZippedSlices Zip(StringSlice first, StringSlice second)
        {
            var splitIndex = CalculateSplitIndex(first, second);

            var (firstHead, firstTail) = Split(first, splitIndex);
            var (_, secondTail) = Split(second, splitIndex);
            return new ZippedSlices(firstHead, firstTail, secondTail);
        }

        private static int CalculateSplitIndex(StringSlice first, StringSlice second)
        {
            var splitIndex = 0;
            using (var thisEnumerator = first.GetEnumerator())
            {
                using (var otherEnumerator = second.GetEnumerator())
                {
                    while (thisEnumerator.MoveNext() && otherEnumerator.MoveNext())
                    {
                        if (thisEnumerator.Current != otherEnumerator.Current)
                        {
                            break;
                        }
                        splitIndex++;
                    }
                }
            }
            return splitIndex;
        }

        private static (StringSlice head, StringSlice tail) Split(StringSlice source, int splitIndex)
        {
            var head = new StringSlice(source.Origin, source.StartIndex, splitIndex);
            var tail = new StringSlice(source.Origin, source.StartIndex + splitIndex, source.Length - splitIndex);
            return (head, tail);
        }
    }
}