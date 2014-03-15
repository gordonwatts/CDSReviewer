using CDSReviewerCore.Data;
using System;
namespace CDSReviewerModels.ServiceInterfaces
{
    /// <summary>
    /// Return a list of papers for a particular search string
    /// </summary>
    interface IPaperSearch
    {
        /// <summary>
        /// Returns a list of papers that satisfy some search string this was created from.
        /// </summary>
        /// <returns></returns>
        IObservable<Tuple<PaperStub, PaperFullInfo>> FindPaper();
    }
}
