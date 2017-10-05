using System.Collections.Generic;
using Jbta.SearchEngine.FileParsing;
using Jbta.SearchEngine.Resources;

namespace Jbta.SearchEngine
{
    public class Settings
    {
        public IEnumerable<string> SupportedFilesExtensions { get; set; } = DefaultSupportedFiles.Extensions;

        public IEnumerable<IFileParser> FileParsers { get; set; }

        public bool GcCollect { get; set; }

        public double CleaUpIntervalMs { get; set; } = 1000;
    }
}