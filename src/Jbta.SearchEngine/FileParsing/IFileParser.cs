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
        /// <param name="encoding">Recognized file encoding or zero if it was not recognized</param>
        /// <returns></returns>
        IEnumerable<ParsedWord> Parse(IFileVersion fileVersion, Encoding encoding = null);
    }
}