using System.Collections.Generic;

namespace Jbta.SearchEngine.Resources
{
    internal static class DefaultSupportedFiles
    {
        public static readonly IEnumerable<string> Extensions = new []
        {
            "txt",
            "log",
            "xml",
            "html",
            "xaml",
            "cs",
            "fs",
            "js",
            "css",
            "sql"
        };
    }
}