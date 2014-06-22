using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.Raw;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
        /// <param name="db">Database where we are storing papers</param>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <param name="version"></param>
        /// <returns>The number 100 when done. Doesn't currenlty correctly return fraction of file downloaded</returns>
        public async Task<IObservable<int>> DownloadPaper(IInternalPaperDB db,
            PaperStub id, PaperFile file, PaperFileVersion version)
        {
            // If the file is done, we are too.
            if (await db.IsFileDownloaded(id, file, version))
                return Observable.Return(100);

            // Create a stream for the file, and then run the download.
            using (var whereToSave = await db.CreatePaperFile(id, file, version))
            {
                var r = RawCDSAccess.SaveDocumentLocally(id.ID, file.FileName, version.VersionNumber, whereToSave);
                await r;
            }
            return Observable.Return(100);
        }
    }
}
