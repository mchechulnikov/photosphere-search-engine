using System.Collections.Generic;
using System.Linq;

namespace Jbta.SearchEngine.Utils
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> e) => e.Where(i => i != null);
    }
}