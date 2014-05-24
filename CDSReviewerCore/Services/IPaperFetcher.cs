using CDSReviewerCore.Data;
using System;

namespace CDSReviewerCore.Services
{
    /// <summary>
    /// Returns information about a paper, given a specific ID, from the main paper archive (e.g. CDS). The ID must
    /// be unique - this doesn't return random infomration about a search string.
    /// </summary>
    public interface IPaperFetcher
    {
        /// <summary>
        /// Returns all files and versions for a particular paper as known by the source
        /// paper service (e.g. CDS).
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        IObservable<PaperFile[]> GetPaperFiles(string paperID);

        /// <summary>
        /// Download the data for a particular file into local storage.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        IObservable<int> DownloadPaper(PaperStub id, PaperFile file, PaperFileVersion version);
    }
}
