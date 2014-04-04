
using Caliburn.Micro.Portable;
using CDSReviewerModels.ServiceInterfaces;
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
        }
    }
}
