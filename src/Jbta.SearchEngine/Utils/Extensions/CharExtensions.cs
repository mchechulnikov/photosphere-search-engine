namespace Jbta.SearchEngine.Utils.Extensions
{
    internal static class CharExtensions
    {
        public static bool EqualsIgnoreCase(this char a, char b)
        {
            return char.ToUpperInvariant(a) == char.ToUpperInvariant(b);
        }
    }
}