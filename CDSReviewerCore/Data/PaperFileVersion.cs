using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDSReviewerCore.Data
{
    /// <summary>
    /// Information for a particular version of a paper stored on CDS.
    /// </summary>
    public class PaperFileVersion
    {
        /// <summary>
        /// The version number represented by this guy.
        /// </summary>
        public int VersionNumber { get; set; }

        /// <summary>
        /// The date this version was uploaded to CDS.
        /// </summary>
        public DateTime VersionDate { get; set; }
    }
}
