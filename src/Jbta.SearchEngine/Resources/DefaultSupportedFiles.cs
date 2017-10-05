using System.Collections.Generic;
using System.Text;

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

        public static readonly IEnumerable<Encoding> Encodings = new[]
        {
            Encoding.UTF7,
            Encoding.UTF8,
            Encoding.Unicode,
            Encoding.BigEndianUnicode,
            Encoding.UTF32,
            Encoding.ASCII
        };
    }
}