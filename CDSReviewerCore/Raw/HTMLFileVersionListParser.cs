using CDSReviewerCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSReviewerCore.Raw
{
    /// <summary>
    /// Parse HTML that comes back from the web server for a list of files.
    /// </summary>
    public static class HTMLFileVersionListParser
    {
        /// <summary>
        /// Parse the code into the structs that will contain file info.
        /// </summary>
        /// <param name="htmltext"></param>
        /// <returns></returns>
        internal static IEnumerable<PaperFile> ParseToFileList(string htmltext)
        {
            throw new NotImplementedException();
        }
    }
}
