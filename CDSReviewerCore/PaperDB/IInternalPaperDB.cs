using CDSReviewerCore.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDSReviewerCore.PaperDB
{
    /// <summary>
    /// The internal paper database engine
    /// </summary>
    interface IInternalPaperDB
    {
        /// <summary>
        /// Add a new paper into the database, or update an existing paper in the database
        /// </summary>
        /// <param name="stub"></param>
        /// <param name="full"></param>
        /// <returns></returns>
        Task Add(PaperStub stub, PaperFullInfo full);

        /// <summary>
        /// Remove a paper from the database.
        /// </summary>
        /// <param name="paperID">The unique ID string for this paper</param>
        /// <returns></returns>
        /// <remarks>
        /// Will fail silently if the ID can't be found.
        /// </remarks>
        Task Remove(string paperID);

        /// <summary>
        /// Return a list of all the papers. Not really sorted by anything. This is very slow,
        /// so don't use it unless absolutely necessary.
        /// </summary>
        /// <returns>List of the paper info (including an empty list if there are none).</returns>
        Task<IEnumerable<Tuple<PaperStub, PaperFullInfo>>> GetFullInformation();

        /// <summary>
        /// Get a list of all the stub information.
        /// </summary>
        /// <returns>A list of the papers (including an empty list if there are none).</returns>
        Task<IEnumerable<PaperStub>> GetStubInformation();

        /// <summary>
        /// Return the full info for the paper info requested.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns>The full info, or a null if it isn't found.</returns>
        Task<PaperFullInfo> GetFullInfoForID(string paperID);

        /// <summary>
        /// Return the paper info for a particular paper.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        Task<Tuple<PaperStub, PaperFullInfo>> GetPaperInfoForID(string paperID);
    }
}
