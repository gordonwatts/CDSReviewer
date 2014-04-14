using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSReviewerCore.Data
{
    /// <summary>
    /// Represents a file on CDS that is attached to this paper.
    /// </summary>
    public class PaperFile
    {
        /// <summary>
        /// Get/Set the name fo the file. This is just the string name (no URI, etc.).
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Get/Set the list of versions associated with this particular file.
        /// </summary>
        public IEnumerable<PaperFileVersion> Versions { get; set; }
    }
}
