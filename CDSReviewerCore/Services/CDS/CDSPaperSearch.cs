using CDSReviewerCore.Data;
using CDSReviewerCore.Raw;
using CDSReviewerCore.ServiceInterfaces;
using System;
using System.Reactive.Linq;

namespace CDSReviewerCore.Services.CDS
{
    /// <summary>
    /// Implement the paper search for CDS.
    /// </summary>
    /// <remarks>We are responsible for a single search for a particular ID. You must create a new version of this
    /// for each search</remarks>
    public class CDSPaperSearch : IPaperSearch
    {
        /// <summary>
        /// We are given a paper ID. Download the proper info from CDS for that when we are requested
        /// </summary>
        /// <param name="id"></param>
        public CDSPaperSearch(int id)
        {
            ID = id;
        }

        /// <summary>
        /// Run the actual search
        /// </summary>
        /// <returns></returns>
        public IObservable<Tuple<PaperStub, PaperFullInfo>> FindPaper()
        {
            return RawCDSAccess.GetDocumentMetadata(ID)
                .Select(md => ConvertToTuple(md));

        }

        /// <summary>
        /// A zero wait task to convert from the MD that comes back from CDS into
        /// an internal format.
        /// </summary>
        /// <param name="md"></param>
        /// <returns></returns>
        private Tuple<PaperStub, PaperFullInfo> ConvertToTuple(IDocumentMetadata md)
        {
            return new Tuple<PaperStub, PaperFullInfo>(
                new PaperStub() { ID = ID.ToString(), Title = md.Title },
                new PaperFullInfo() { Abstract = md.Abstract, Authors = md.Authors }
            );
        }

        /// <summary>
        /// Get back the ID that we are looking for.
        /// </summary>
        /// <remarks>This is mostly to make testing "easy".</remarks>
        public int ID { get; private set; }
    }
}
