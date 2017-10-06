using System.Collections.Generic;

namespace Jbta.SearchEngine.Resources
{
    internal static class DefaultSupportedFiles
    {
        public static readonly IEnumerable<string> Extensions = new []
        {
            "txt",
            "log",
            "cs",
            "fs",
            "js",
            "css",
            "sql"
        };
    }
}