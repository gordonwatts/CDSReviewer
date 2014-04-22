using CDSReviewerCore.Services.CDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSReviewerCore.Services
{
    /// <summary>
    /// Responsible for mapping an ID into a particular type of service.
    /// </summary>
    public class PaperIDFetchParser : IPaperIDFetchParser
    {
        /// <summary>
        /// Return the proper service for a paper ID
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        public IObservable<IPaperDataFetcher> GetFetcher(string paperID)
        {
            // we currently only know about CDS
            return Observable.Return(new CDSPaperDataFetcher(paperID));
        }
    }
}
