using Caliburn.Micro;
using System;

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
        public PaperFileViewModel(string fname, int version, DateTime date)
        {
            FileName = fname;
            Version = version;
            FileDate = date;
        }

        /// <summary>
        /// Get the filename that we represent
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Get the filename that we represent
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Get the date that this version was uploaded to CDS
        /// </summary>
        public DateTime FileDate { get; private set; }

        public bool IsDownloaded { get; set; }
    }
}
