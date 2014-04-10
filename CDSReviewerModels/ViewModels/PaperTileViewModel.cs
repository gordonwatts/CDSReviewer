using Caliburn.Micro;
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
    public class PaperTileViewModel : PropertyChangedBase
    {
        public PaperTileViewModel(INavService nav, PaperStub basicInfo, PaperFullInfo fullInfo)
        {
            this._basicInfo = basicInfo;
            this._fullInfo = fullInfo;

            Observable.Return(_basicInfo.Title)
                .ToPropertyCM(this, x => x.PaperTitle, out _TitleOAPH, "");
            Observable.Return(_fullInfo.Abstract)
                .ToPropertyCM(this, x => x.Abstract, out _AbstractOAPH, "");
        }

        readonly PaperStub _basicInfo;

        readonly PaperFullInfo _fullInfo;

        /// <summary>
        /// Get the paperID - not meant for using in UI since it can't update!
        /// </summary>
        public string PaperID
        {
            get { return _basicInfo.ID; }
        }

        /// <summary>
        /// The title for the paper
        /// </summary>
        public string PaperTitle
        {
            get { return _TitleOAPH.Value; }
        }
        private ObservableAsPropertyHelper<string> _TitleOAPH;

        /// <summary>
        /// The abstract for a paper
        /// </summary>
        public string Abstract
        {
            get { return _AbstractOAPH.Value; }
        }
        private ObservableAsPropertyHelper<string> _AbstractOAPH;

        /// <summary>
        /// Return title as a string representation. Mostly for hacking and
        /// getting things working quickly.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return PaperTitle;
        }
    }
}
