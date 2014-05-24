using CDSReviewerCore.Data;
using CDSReviewerCore.Raw;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace CDSReviewerCore.Services.CDS
{
    /// <summary>
    /// Returns the paper data required for a cds paper
    /// </summary>
    public class CDSPaperDataFetcher : IPaperFetcher
    {
        /// <summary>
        /// Return a list of the paper files associated with a single paper.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        public IObservable<PaperFile[]> GetPaperFiles(string paperID)
        {
            return RawCDSAccess.GetDocumentFiles(ParseIDString(paperID)).Select(x => x.ToArray());
        }

        /// <summary>
        /// Convert an ID string into an ID string that can be used to fetch an item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private static int ParseIDString(string id)
        {
            return int.Parse(id);
        }

        /// <summary>
        /// Download a file into local storage. If we already have it locally, then do nothing.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public IObservable<int> DownloadPaper(PaperStub id, PaperFile file, PaperFileVersion version)
        {
            // Get the location where we will store this file

            // If the file is there, we should return that

            // Download the file.

            // Return the location of the file for use by others!
            throw new NotImplementedException();
        }
    }
}
