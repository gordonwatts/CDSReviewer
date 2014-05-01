using Caliburn.Micro;
using CDSReviewerCore.Data;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// Represents a paper file view model. This is meant to display a single version of a file
    /// self contained.
    /// </summary>
    public class PaperFileViewModel : PropertyChangedBase
    {
        /// <summary>
        /// Initialize the view model with the appropriate values
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="version"></param>
        /// <param name="date"></param>
        public PaperFileViewModel(string paperID, PaperFile file, PaperFileVersion version)
        {
            FileName = file.FileName;
            Version = version.VersionNumber;
            FileDate = version.VersionDate;
        }

        /// <summary>
        /// Get the filename that we represent
        /// </summary>
        public string FileName {get; private set;}

        /// <summary>
        /// Get the filename that we represent
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Get the date that this version was uploaded to CDS
        /// </summary>
        public DateTime FileDate { get; private set; }
    }
}
