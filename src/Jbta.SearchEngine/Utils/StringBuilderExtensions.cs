using System.Text;

namespace Jbta.SearchEngine.Utils
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder TrimEndingPunctuationChars(this StringBuilder sb)
        {
            var removeIndex = 0;
            for (var i = sb.Length - 1; i > 0; i--)
            {
                if (char.IsPunctuation(sb[i]))
                {
                    removeIndex = i;
                }
                else
                {
                    break;
                }
            }

            if (removeIndex != 0)
            {
                sb.Remove(removeIndex, sb.Length - removeIndex);
            }
            return sb;
        }
    }
}