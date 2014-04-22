using CDSReviewerCore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSReviewerCore.Services
{
    /// <summary>
    /// Fetch the data for a particular paper given by some ID.
    /// </summary>
    public interface IPaperDataFetcher
    {
        /// <summary>
        /// Returns the papers associated with a particular ID.
        /// </summary>
        /// <returns></returns>
        IObservable<PaperFile> GetPapers();
    }
}
