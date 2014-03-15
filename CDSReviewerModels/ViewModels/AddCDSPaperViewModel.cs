using Caliburn.Micro.Portable;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// View model for adding a paper to the archive. Takes care of look up and
    /// verification after user enters a "search" string.
    /// </summary>
    public class AddCDSPaperViewModel : ViewModelBase
    {
        /// <summary>
        /// Pass down the navagation service
        /// </summary>
        /// <param name="nav"></param>
        public AddCDSPaperViewModel(INavService nav)
            : base(nav)
        { }

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

        /// <summary>
        /// The title that is found from a search.
        /// </summary>
        public string Title
        {
            get { return _Title; }
            private set { this.NotifyAndSetIfChanged(ref _Title, value); }
        }
        private string _Title;

        /// <summary>
        /// The abstract found from a search
        /// </summary>
        public string Abstract
        {
            get { return _Abstract; }
            private set { this.NotifyAndSetIfChanged(ref _Abstract, value); }
        }
        private string _Abstract;

        /// <summary>
        /// The authors
        /// </summary>
        public string Authors
        {
            get { return _Authors; }
            private set { this.NotifyAndSetIfChanged(ref _Authors, value); }
        }
        private string _Authors;
    }
}
