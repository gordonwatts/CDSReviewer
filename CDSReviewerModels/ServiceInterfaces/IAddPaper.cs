using CDSReviewerCore.Data;
using System.Threading.Tasks;

namespace CDSReviewerModels.ServiceInterfaces
{
    public interface IAddPaper
    {
        /// <summary>
        /// Add a paper to the central database. Do nothing if the paper
        /// is already added.
        /// </summary>
        /// <param name="stubInfo">Half the info needed for the paper</param>
        /// <param name="fullInfo">Detailed info for the paper</param>
        /// <remarks>This should only return when the paper has been added to the local database. Whatever time it takes
        /// to reflect up to the main repo we don't care</remarks>
        Task AddPaperLocally(PaperStub stubInfo, PaperFullInfo fullInfo);
    }
}
