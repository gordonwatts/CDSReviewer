using Caliburn.Micro;
using Caliburn.Micro.Portable;

namespace CDSReviewewrModels.ViewModels
{
    /// <summary>
    /// The home page view model
    /// </summary>
    public class HomePageViewModel : Screen
    {
        /// <summary>
        /// Track the navagation service we will be using to move around the app.
        /// </summary>
        private readonly INavService _nav;

        /// <summary>
        /// Initialize with the navigation service.
        /// </summary>
        /// <param name="nav"></param>
        public HomePageViewModel(INavService nav)
        {
            _nav = nav;
        }

        /// <summary>
        /// Move away to the add new document view.
        /// </summary>
        public void CmdAdd()
        {
            _nav.NavigateToViewModel<AddCDSPaperViewModel>();
        }
    }
}
