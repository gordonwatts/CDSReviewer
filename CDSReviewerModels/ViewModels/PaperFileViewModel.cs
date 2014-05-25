using Caliburn.Micro;
using CDSReviewerCore.Data;
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
        /// Return the paper file we are attached to.
        /// </summary>
        public PaperFile _file { get; private set; }

        /// <summary>
        /// Return the version info we are attached to.
        /// </summary>
        public PaperFileVersion _version { get; private set; }

        /// <summary>
        /// Initialize the view model with the appropriate values
        /// </summary>
        /// <param name="fname"></param>
        /// <param name="version"></param>
        /// <param name="date"></param>
        public PaperFileViewModel(PaperFile file, PaperFileVersion version)
        {
            _file = file;
            _version = version;

            FileName = file.FileName;
            Version = version.VersionNumber;
            FileDate = version.VersionDate;
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

        /// <summary>
        /// Set when the file download is complete.
        /// </summary>
        public bool IsDownloaded { get; set; }

        /// <summary>
        /// Denotes the fraction of the file that is downloaded when a download is in progress.
        /// </summary>
        public int DownloadFraction { get; set; }
    }
}
