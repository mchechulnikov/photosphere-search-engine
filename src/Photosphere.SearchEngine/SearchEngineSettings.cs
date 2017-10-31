using System.Collections.Generic;
using Photosphere.SearchEngine.FileParsing;
using Photosphere.SearchEngine.Resources;

namespace Photosphere.SearchEngine
{
    public class SearchEngineSettings
    {
        /// <summary>
        /// List of supported files extensions. Files with other extensions will be ignored while parsing
        /// </summary>
        public IEnumerable<string> SupportedFilesExtensions { get; set; } = DefaultSupportedFiles.Extensions;

        /// <summary>
        /// List of file parsers. The parser will be selected for the file by extension
        /// </summary>
        public IEnumerable<IFileParser> FileParsers { get; set; }

        /// <summary>
        /// Manages whether to collect garbage after the index is cleared
        /// </summary>
        public bool GcCollect { get; set; }

        /// <summary>
        /// Clean up index interval in milliseconds
        /// </summary>
        public double CleaUpIntervalMs { get; set; } = 5000;
    }
}