using CDSReviewerCore.ServiceInterfaces;
using System;
using System.Reactive.Linq;
using System.Text.RegularExpressions;

namespace CDSReviewerCore.Services.CDS
{
    /// <summary>
    /// Implement the search string parser for CDS
    /// </summary>
    public class CDSSearchStringParser : ISearchStringParser
    {
        /// <summary>
        /// Parse the incoming string for either an ID or a valid CDS url.
        /// </summary>
        /// <param name="searchstring"></param>
        /// <returns>A list of searchers</returns>
        /// <remarks>None of this work should occur on another thread - so this should be easy</remarks>
        public IObservable<IPaperSearch> GetPaperFinders(string searchstring)
        {
            // Is it an ID? That is just a collection of integers
            int v;
            if (int.TryParse(searchstring, out v))
            {
                return Observable.Return(new CDSPaperSearch(v));
            }

            // Is it a valid URL?
            var rFinder = new Regex("^https://cds.cern.ch/record/(?<id>[0-9]+)(\\?|/.*)$");
            var g = rFinder.Match(searchstring);
            if (g.Success)
            {
                return Observable.Return(new CDSPaperSearch(int.Parse(g.Groups["id"].Value)));
            }

            // Nope. Bad - return nothing.
            return Observable.Empty<IPaperSearch>();
        }
    }
}
