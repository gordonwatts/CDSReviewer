using Caliburn.Micro.Portable;
using Caliburn.Micro.ReactiveUI;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// Display detailed infomration for a single paper, including access
    /// to all special things (like "open").
    /// </summary>
    public class PaperViewModel : ViewModelBase
    {
        /// <summary>
        /// Initialize the paper view model
        /// </summary>
        /// <param name="nav"></param>
        public PaperViewModel(INavService nav, IInternalPaperDB localI, IPaperIDFetchParser paperFinder)
            : base(nav)
        {

            // When the paper ID is set, kick off a lookup for the paper.

            _findPaper = ReactiveCommand.Create(
                idString => Observable.Return(idString as string)
                    .SelectMany(id => Observable.FromAsync(_ => localI.GetPaperInfoForID(id)))
                );

            // When the lookup is done, fill all the various properties
            _findPaper
                .Select(x => x.Item1.Title)
                .ToPropertyCM(this, x => x.PaperTitle, out _TitleOAPH, "");
            _findPaper
                .Select(x => x.Item2.Abstract)
                .ToPropertyCM(this, x => x.Abstract, out _AbstractOAPH, "");
            _findPaper
                .Select(x => x.Item2.Authors)
                .ToPropertyCM(this, x => x.Authors, out _AuthorsOAPH, new string[0]);
            _findPaper
                .Select(x => x.Item2.Files)
                .Where(x => x != null)
                .Select(x => new ObservableCollection<PaperFileViewModel>(x.Select(MostRecentFileVersionVM)))
                .ToPropertyCM(this, x => x.PaperVersions, out _PaperVersionsOAPH, new ObservableCollection<PaperFileViewModel>());

            // When we have an update to the string property guy, off we go!
            this.ObservableForProperty(p => p.PaperID)
                .Subscribe(x => _findPaper.Execute(x.Value));

            // When the initial paper lookup is done, re-fetch files from
            // CDS.
            _findPaper
                .SelectMany(x => paperFinder.GetFetcher(PaperID))
                .SelectMany(x => x.GetPapers())
                .ToArray()
                .Subscribe(x => MergeWithObservable(x));
        }

        /// <summary>
        /// Merge a new set of files with the observable collection that already exists. Also,
        /// update our central database if anything has changed.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private object MergeWithObservable(PaperFile[] x)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Return a vm for the papers' most recent VM.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private PaperFileViewModel MostRecentFileVersionVM(PaperFile aFile)
        {
            var mostRecentVersion = aFile.Versions.OrderByDescending(x => x.VersionNumber).First();
            return new PaperFileViewModel(
                aFile.FileName,
                mostRecentVersion.VersionNumber, mostRecentVersion.VersionDate
            );
        }

        /// <summary>
        /// When activated, goes after the paper lookup
        /// </summary>
        private readonly ReactiveCommand<Tuple<PaperStub, PaperFullInfo>> _findPaper;

        /// <summary>
        /// Get/Set ID of the paper we are to display. We use this as a direct
        /// key into the local database.
        /// </summary>
        public string PaperID
        {
            get { return _PaperID; }
            set { this.NotifyAndSetIfChanged(ref _PaperID, value); }
        }
        string _PaperID;

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
        /// The list of paper verisons
        /// </summary>
        public ObservableCollection<PaperFileViewModel> PaperVersions
        {
            get { return _PaperVersionsOAPH.Value; }
        }
        private ObservableAsPropertyHelper<ObservableCollection<PaperFileViewModel>> _PaperVersionsOAPH;
    }
}
