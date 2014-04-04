
using Caliburn.Micro.Portable;
using Caliburn.Micro.ReactiveUI;
using CDSReviewerCore.Data;
using CDSReviewerModels.ServiceInterfaces;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// Show a list of papers that we've been given to look at. Just their titles, etc (easy info) so that
    /// a user can select one and do various things to it.
    /// </summary>
    public class PaperListViewModel : ViewModelBase
    {
        /// <summary>
        /// Get setup with the list of papers we need to show.
        /// </summary>
        /// <param name="nav"></param>
        /// <param name="paperDB"></param>
        public PaperListViewModel(INavService nav, ILocalDBAccess paperDB)
            : base(nav)
        {
            Observable.FromAsync(paperDB.GetAllPapers)
                .Select(x => new ObservableCollection<Tuple<PaperStub, PaperFullInfo>>(x))
                .ToPropertyCM(this, x => x.PaperList, out _PaperListOAPH, null);
        }

        /// <summary>
        /// The list of papers this view model is showing
        /// </summary>
        public ObservableCollection<Tuple<PaperStub, PaperFullInfo>> PaperList
        {
            get { return _PaperListOAPH.Value; }
        }
        private ObservableAsPropertyHelper<ObservableCollection<Tuple<PaperStub, PaperFullInfo>>> _PaperListOAPH;
    }
}
