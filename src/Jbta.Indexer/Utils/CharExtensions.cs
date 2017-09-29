namespace Jbta.Indexing.Utils
{
    internal static class CharExtensions
    {
        public static bool EqualsIgnoreCase(this char a, char b)
        {
            return char.ToUpperInvariant(a) == char.ToUpperInvariant(b);
        }
    }
}