using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDSReviewerCore.Services
{
    /// <summary>
    /// Access various status things about a file that is being downloaded.
    /// </summary>
    public interface IDownloadProgressInfo
    {
        /// <summary>
        /// True or false depending if the download is pending or
        /// is actually running.
        /// </summary>
        IObservable<bool> IsDownloading {get; set;}

        /// <summary>
        /// From zero to 100 the % of the file that has been downloaded.
        /// </summary>
        IObservable<int> PercentDone { get; set; }
    }
}
