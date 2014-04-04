using Caliburn.Micro.Portable;
using Caliburn.Micro.ReactiveUI;
using CDSReviewerCore.Data;
using ReactiveUI;
using System.Reactive.Linq;

namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// Simple view model that provates read-only data to a user control that displays
    /// some very basic infomration about a paper.
    /// </summary>
    public class PaperTileViewModel : ViewModelBase
    {
        public PaperTileViewModel(INavService nav, PaperStub basicInfo, PaperFullInfo fullInfo)
            : base(nav)
        {
            this._basicInfo = basicInfo;
            this._fullInfo = fullInfo;

            Observable.Return(_basicInfo.Title)
                .ToPropertyCM(this, x => x.Title, out _TitleOAPH, "");
        }

        readonly PaperStub _basicInfo;

        readonly PaperFullInfo _fullInfo;

        /// <summary>
        /// The title for the paper
        /// </summary>
        public string Title
        {
            get { return _TitleOAPH.Value; }
        }
        private ObservableAsPropertyHelper<string> _TitleOAPH;
    }
}
