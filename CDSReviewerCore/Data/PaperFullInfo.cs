
using System.Collections.Generic;
namespace CDSReviewerCore.Data
{
    /// <summary>
    /// The full information for a paper - excluding everything that is in the stub info.
    /// </summary>
    public class PaperFullInfo
    {
        /// <summary>
        /// Paper abstract
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// List of authors
        /// </summary>
        public string[] Authors { get; set; }

        /// <summary>
        /// Get/Set the list of files that CDS (and us) know about.
        /// </summary>
        public PaperFile[] Files { get; set; }
    }
}
