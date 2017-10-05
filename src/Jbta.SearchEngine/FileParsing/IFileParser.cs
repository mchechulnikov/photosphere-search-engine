using System.Collections.Generic;
using System.Text;

namespace Jbta.SearchEngine.FileParsing
{
    public interface IFileParser
    {
        /// <summary>
        /// Extensions of files, which can be parsed by this parser
        /// </summary>
        IEnumerable<string> FileExtensions { get; }

        /// <summary>
        /// Parse file by specific version
        /// </summary>
        /// <param name="fileVersion">Specific file version</param>
        /// <param name="encoding">Detected file encoding or null if it's not to be detected</param>
        /// <returns></returns>
        IEnumerable<ParsedWord> Parse(IFileVersion fileVersion, Encoding encoding);
    }
}