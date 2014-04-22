using CDSReviewerCore.Data;
using CDSReviewerCore.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSReviewerCore.Services.CDS
{
    /// <summary>
    /// Returns the paper data required for a cds paper
    /// </summary>
    class CDSPaperDataFetcher  : IPaperDataFetcher
    {
        /// <summary>
        /// Initalize with the paper we will be responsible for
        /// </summary>
        /// <param name="paperID"></param>
        public CDSPaperDataFetcher(string paperID)
        {
            _paperID = int.Parse(paperID);
        }

        /// <summary>
        /// Return the paper file data
        /// </summary>
        /// <returns></returns>
        public IObservable<PaperFile> GetPapers()
        {
            return RawCDSAccess.GetDocumentFiles(_paperID).SelectMany(x => x);
        }

        /// <summary>
        /// Cache the paper data info
        /// </summary>
        private readonly int _paperID;
    }
}
