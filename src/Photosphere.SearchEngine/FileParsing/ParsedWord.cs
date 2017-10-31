namespace Photosphere.SearchEngine.FileParsing
{
    public class ParsedWord
    {
        public ParsedWord(string word, WordEntry entry)
        {
            Word = word;
            Entry = entry;
        }

        public string Word { get; }

        public WordEntry Entry { get; }
    }
}