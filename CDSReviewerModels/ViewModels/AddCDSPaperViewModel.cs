using Caliburn.Micro;
using Caliburn.Micro.Portable;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// View model for adding a paper to the archive. Takes care of look up and
    /// verification after user enters a "search" string.
    /// </summary>
    public class AddCDSPaperViewModel : Screen
    {
        /// <summary>
        /// Get/Set the CDS lookup string. Used to do the search for the document.
        /// - Currently supports only a URL.
        /// </summary>
        public string CDSLookupString
        {
            get { return _CDSLookupString; }
            set { this.NotifyAndSetIfChanged(ref _CDSLookupString, value); }
        }
        string _CDSLookupString;
    }
}
