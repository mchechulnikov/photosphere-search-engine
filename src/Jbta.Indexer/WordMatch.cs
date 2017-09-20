namespace Jbta.Indexing
{
    public class WordMatch
    {
        public WordMatch(string fullFileName, PositionInFile position)
        {
            FullFileName = fullFileName;
            Position = position;
        }

        public string FullFileName { get; }

        public PositionInFile Position { get; }
    }
}