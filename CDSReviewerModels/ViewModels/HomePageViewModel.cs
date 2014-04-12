using Caliburn.Micro.Portable;
using Caliburn.Micro.ReactiveUI;
using CDSReviewerCore.Data;
using CDSReviewerCore.PaperDB;
using CDSReviewerCore.ViewModels;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// The home page view model
    /// </summary>
    public class HomePageViewModel : ViewModelBase
    {
        /// <summary>
        /// Initialize with the navigation service.
        /// </summary>
        /// <param name="nav"></param>
        public HomePageViewModel(INavService nav, IInternalPaperDB paperDB)
            : base(nav)
        {
            // Load up the list of papers to display

            _paperListRaw = new ObservableCollection<Tuple<PaperStub, PaperFullInfo>>();
            Observable.FromAsync(paperDB.GetFullInformation)
                .Select(x => new ObservableCollection<Tuple<PaperStub, PaperFullInfo>>(x))
                .Select(x =>
                {
                    _paperListRaw = x;
                    var r = _paperListRaw.CreateDerivedCollection(t => new PaperTileViewModel(nav, t.Item1, t.Item2));
                    return r;
                })
                .ToPropertyCM(this, x => x.PaperList, out _PaperListOAPH, _paperListRaw.CreateDerivedCollection(t => new PaperTileViewModel(nav, t.Item1, t.Item2)));

            // Setup the navagate command to move away from this display
            NavigateToPaperTile = new ReactiveCommand<PaperTileViewModel>(Observable.Return(true), o => Observable.Return(o as PaperTileViewModel), RxApp.MainThreadScheduler);
            NavigateToPaperTile.Subscribe(pt =>
                _nav.UriForViewModel<PaperViewModel>()
                .WithParam(x => x.PaperID, pt.PaperID)
                .Navigate()
                );
        }

        private ObservableCollection<Tuple<PaperStub, PaperFullInfo>> _paperListRaw;

        /// <summary>
        /// The list of papers this view model is showing
        /// </summary>
        public IReactiveDerivedList<PaperTileViewModel> PaperList
        {
            get { return _PaperListOAPH.Value; }
        }
        private ObservableAsPropertyHelper<IReactiveDerivedList<PaperTileViewModel>> _PaperListOAPH;

        /// <summary>
        /// Command to move to a new paper tile display.
        /// </summary>
        public ReactiveCommand<PaperTileViewModel> NavigateToPaperTile { get; private set; }

        /// <summary>
        /// Move away to the add new document view.
        /// </summary>
        public void CmdAdd()
        {
            _nav.NavigateToViewModel<AddCDSPaperViewModel>();
        }

        /// <summary>
        /// When the view is attached, see if it implements our call back for final setup
        /// </summary>
        /// <param name="view"></param>
        /// <param name="context"></param>
        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
            if (view is IHomePageViewCallback)
            {
                (view as IHomePageViewCallback).FinalizeVMWiring(this);
            }
        }
    }
}
