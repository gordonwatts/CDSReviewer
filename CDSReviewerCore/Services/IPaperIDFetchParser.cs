using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSReviewerCore.Services
{
    /// <summary>
    /// Given a paper ID, return a fetcher
    /// </summary>
    public interface IPaperIDFetchParser
    {
        /// <summary>
        /// Return the proper paper fetcher given a paper ID.
        /// </summary>
        /// <param name="paperID"></param>
        /// <returns></returns>
        IObservable<IPaperDataFetcher> GetFetcher(string paperID);
    }
}
