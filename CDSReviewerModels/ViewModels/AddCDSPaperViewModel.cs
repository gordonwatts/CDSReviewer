using Caliburn.Micro.Portable;
using Caliburn.Micro.ReactiveUI;
using CDSReviewerModels.ServiceInterfaces;
using ReactiveUI;
using System;
using System.Reactive.Linq;

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
        public AddCDSPaperViewModel(INavService nav, ISearchStringParser parser)
            : base(nav)
        {
            _searchParser = parser;

            // When we run, look for the first paper, and route its output to our author, etc.
            _executeSearch = ReactiveCommand.Create();
            var paper = _executeSearch
                .Where(x => x is string)
                .SelectMany(x => _searchParser.GetPaperFinders(x as string))
                .SelectMany(x => x.FindPaper());

            paper
                .Select(x => x.Item1.Title)
                .ToPropertyCM(this, x => x.Title, out _TitleOAPH, "");
            paper
                .Select(x => x.Item2.Abstract)
                .ToPropertyCM(this, x => x.Abstract, out _AbstractOAPH, "");
            paper
                .Select(x => x.Item2.Authors)
                .ToPropertyCM(this, x => x.Authors, out _AuthorsOAPH, new string[0]);

            // When the user types in something, we need to trigger a search
            this.ObservableForProperty(p => p.CDSLookupString)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.TaskpoolScheduler)
                .Select(x => x.Value)
                .DistinctUntilChanged()
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Subscribe(x => _executeSearch.Execute(x));

            // When the search is running, make sure the search in progress indicator is off.
            _executeSearch
                .Select(x => true)
                .Merge(paper.Select(x => false))
                .ToPropertyCM(this, x => x.SearchInProgress, out _SearchInProgressOAPH, false);
        }

        /// <summary>
        /// Run the search
        /// </summary>
        private readonly ReactiveCommand<object> _executeSearch;

        /// <summary>
        /// The paper searcher, we use it when the user is ready to search.
        /// </summary>
        private readonly ISearchStringParser _searchParser;

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
            get { return _TitleOAPH.Value; }
        }
        private ObservableAsPropertyHelper<string> _TitleOAPH;

        /// <summary>
        /// The abstract found from a search
        /// </summary>
        public string Abstract
        {
            get { return _AbstractOAPH.Value; }
        }
        private ObservableAsPropertyHelper<string> _AbstractOAPH;

        /// <summary>
        /// The author list
        /// </summary>
        public string[] Authors
        {
            get { return _AuthorsOAPH.Value; }
        }
        private ObservableAsPropertyHelper<string[]> _AuthorsOAPH;

        /// <summary>
        /// Get the state of a search. In progress, or not?
        /// </summary>
        public bool SearchInProgress
        {
            get { return _SearchInProgressOAPH.Value; }
        }
        private ObservableAsPropertyHelper<bool> _SearchInProgressOAPH;
    }
}
