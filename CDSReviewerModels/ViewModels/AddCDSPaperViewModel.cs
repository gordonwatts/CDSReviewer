﻿using Caliburn.Micro.Portable;
using Caliburn.Micro.ReactiveUI;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.ServiceInterfaces;
using ReactiveUI;
using System;
using System.Reactive;
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
        public AddCDSPaperViewModel(INavService nav, ISearchStringParser parser, IInternalPaperDB adder)
            : base(nav)
        {
            _searchParser = parser;
            _paperAdder = adder;

            // When we run, look for the first paper, and route its output to our author, etc.
            _executeSearch = new ReactiveCommand<Tuple<PaperStub, PaperFullInfo>>(
                Observable.Return(true),
                searchString => Observable.Return(searchString)
                    .Where(x => x is string)
                    .SelectMany(x => _searchParser.GetPaperFinders(x as string))
                    .SelectMany(x => x.FindPaper().Catch(Observable.Empty<Tuple<PaperStub, PaperFullInfo>>()))
                );

            // When the user types in something, we need to trigger a search
            var startSearch = this.ObservableForProperty(p => p.CDSLookupString)
                .Throttle(TimeSpan.FromMilliseconds(500), RxApp.TaskpoolScheduler)
                .Select(x => x.Value)
                .DistinctUntilChanged()
                .Where(x => !string.IsNullOrWhiteSpace(x));

            startSearch
                .Subscribe(x => _executeSearch.Execute(x));

            var emptyTitle = startSearch
                .Select(_ => "");
            var emptyAbstract = startSearch
                .Select(_ => "");
            var emptyAuthors = startSearch
                .Select(_ => new string[0]);

            // Several places where we will be wanting to update the
            // various properties we are going after...
            var titleSet = _executeSearch
                .Select(x => x.Item1.Title)
                .Merge(emptyTitle);
            titleSet
                .ToPropertyCM(this, x => x.PaperTitle, out _TitleOAPH, "");

            _executeSearch
                .Select(x => x.Item2.Abstract)
                .Merge(emptyAbstract)
                .ToPropertyCM(this, x => x.Abstract, out _AbstractOAPH, "");

            _executeSearch
                .Select(x => x.Item2.Authors)
                .Merge(emptyAuthors)
                .ToPropertyCM(this, x => x.Authors, out _AuthorsOAPH, new string[0]);

            _executeSearch
                .Subscribe(x =>
                {
                    _paperStub = x.Item1;
                    _paperFullInfo = x.Item2;
                });


            // When the search is running, make sure the search in progress indicator is off.
            _executeSearch
                .IsExecuting
                .ToPropertyCM(this, x => x.SearchInProgress, out _SearchInProgressOAPH, false);

            // We can only add something when all searches are "good"
            var cmdGood = titleSet
                .Select(x => x != "");
            _addButtonCommand = new ReactiveCommand<Unit>(cmdGood, _ => Observable.FromAsync(t => _paperAdder.Add(_paperStub, _paperFullInfo)));
            _addButtonCommand
                .Subscribe(_ =>
                {
                    nav
                        .UriForViewModel<PaperViewModel>()
                        .WithParam(x => x.PaperID, _paperStub.ID)
                        .Navigate();
                });

        }

        /// <summary>
        /// Cache the currently found paper's info
        /// </summary>
        private PaperStub _paperStub;

        /// <summary>
        /// Cache the currently found paper's full info
        /// </summary>
        private PaperFullInfo _paperFullInfo;

        /// <summary>
        /// Run when we can add a button.
        /// </summary>
        public ReactiveCommand<Unit> AddButtonCommand { get { return _addButtonCommand; } }
        public readonly ReactiveCommand<Unit> _addButtonCommand;

        /// <summary>
        /// Run the search
        /// </summary>
        private readonly ReactiveCommand<Tuple<PaperStub, PaperFullInfo>> _executeSearch;

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
        public string PaperTitle
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

        /// <summary>
        /// The interface to add the paper to the central database
        /// </summary>
        private IInternalPaperDB _paperAdder;
    }
}
