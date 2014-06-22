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
using System.Threading.Tasks;

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
        public PaperViewModel(INavService nav, IInternalPaperDB localI, IPaperFetcher paperFinder, IOSFileHandler fileIO)
            : base(nav)
        {
            // Save a few things for later use
            _localI = localI;

            // When the paper ID is set, kick off a lookup for the paper.

            _findPaper = new ReactiveCommand<Tuple<PaperStub, PaperFullInfo>>(Observable.Return(true),
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

            // Papers will be updated from two sources, the origianl soruce when we start
            // up, and then an update that can come in from other sources.
            var papers1 = _findPaper
                .Select(x => x.Item2.Files)
                .Where(x => x != null);

            var papers2 = _findPaper
                .SelectMany(x => paperFinder.GetPaperFiles(PaperID))
                .Where(x => x != null)
                .SelectMany(x => Observable.FromAsync(_ => MergeWithObservable(x)));

            papers1.Merge(papers2)
                .Select(x => new ObservableCollection<PaperFileViewModel>(x.Select(MostRecentFileVersionVM)))
                .ToPropertyCM(this, x => x.PaperVersions, out _PaperVersionsOAPH, new ObservableCollection<PaperFileViewModel>());

            _findPaper
                .Subscribe(x => { _paperStub = x.Item1; });

            // When we have an update to the string property guy, off we go!
            this.ObservableForProperty(p => p.PaperID)
                .Subscribe(x => _findPaper.Execute(x.Value));

            // And when the user clicks on a particular paper, we need to do some work!
            // Open the file if it is already there, otherwise start the download.
            OpenPaperVersion = new ReactiveCommand<PaperFileViewModel>(Observable.Return(true), o => Observable.Return(o as PaperFileViewModel), RxApp.MainThreadScheduler);

            OpenPaperVersion
                .Where(x => IsDownloaded(x).Wait(1000))
                .Subscribe(x => OpenFile(x, fileIO));

            OpenPaperVersion
                .Where(x => !IsDownloaded(x).Wait(1000))
                .Subscribe(x => StartFileDownload(x, paperFinder));
        }

        /// <summary>
        /// Hold onto the paper stub
        /// </summary>
        private PaperStub _paperStub;

        /// <summary>
        /// Download the file from the main repository for papers.
        /// </summary>
        /// <param name="paperVM">The paper we should download</param>
        /// <returns></returns>
        private async Task StartFileDownload(PaperFileViewModel paperVM, IPaperFetcher fetcher)
        {
            var progress = await fetcher.DownloadPaper(_localI, _paperStub, paperVM._file, paperVM._version);
            progress
                .Subscribe(
                    p => { paperVM.DownloadFraction = p; },
                    () => { paperVM.DownloadFraction = 100; paperVM.IsDownloaded = true; }
                );
        }

        /// <summary>
        /// Trigger an opening of the file
        /// </summary>
        /// <param name="x"></param>
        /// <param name="fileIO"></param>
        /// <returns></returns>
        private void OpenFile(PaperFileViewModel x, IOSFileHandler fileIO)
        {
            fileIO.OpenFile();
        }

        /// <summary>
        /// Check to see if the file has been downloaded already
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private async Task<bool> IsDownloaded(PaperFileViewModel x)
        {
            return await _localI.IsFileDownloaded(_paperStub, x._file, x._version);
        }

        /// <summary>
        /// Fired from the UI when a particular paper file view is clicked... We need to open the file!
        /// </summary>
        public ReactiveCommand<PaperFileViewModel> OpenPaperVersion { get; private set; }

        /// <summary>
        /// Merge a new set of files with the observable collection that already exists. Also,
        /// update our central database if anything has changed.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private async Task<PaperFile[]> MergeWithObservable(PaperFile[] x)
        {
            await _localI.Merge(PaperID, x);
            return x;
        }

        /// <summary>
        /// Return a vm for the papers' most recent VM.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private PaperFileViewModel MostRecentFileVersionVM(PaperFile aFile)
        {
            var mostRecentVersion = aFile.Versions.OrderByDescending(x => x.VersionNumber).First();
            return new PaperFileViewModel(aFile, mostRecentVersion) { IsDownloaded = _localI.IsFileDownloaded(_paperStub, aFile, mostRecentVersion).Wait(1000) };
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
        private IInternalPaperDB _localI;
    }
}
