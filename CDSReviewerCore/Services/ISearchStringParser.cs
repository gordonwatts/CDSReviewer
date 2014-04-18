using System;
namespace CDSReviewerCore.ServiceInterfaces
{
    /// <summary>
    /// How to parse a search string that the user enters, and how to resolve it
    /// to a search guy or paper or papers.
    /// </summary>
    /// <remarks>
    /// Fort this early version it is pretty simplistic what it returns!
    /// </remarks>
    public interface ISearchStringParser
    {
        /// <summary>
        /// Returns a list of search guys that can look for a particular paper using a search string.
        /// It is legal to pass a paper ID as a search string, in which case the unique paper info must
        /// be returned.
        /// </summary>
        /// <param name="searchstring">A search string or ID</param>
        /// <returns></returns>
        IObservable<IPaperSearch> GetPaperFinders(string searchstring);
    }
}
