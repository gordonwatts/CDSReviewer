using CDSReviewerCore.ViewModels;
using CDSReviewerModels.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CDSReviewerWS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePageView : Page, IHomePageViewCallback
    {
        public HomePageView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// when the view is finished being hooked up, then we do the final wiring for the item clicked event.
        /// </summary>
        /// <param name="homePageViewModel"></param>
        public void FinalizeVMWiring(HomePageViewModel myVM)
        {
            PaperList.ItemClick += (s, args) => myVM.NavigateToPaperTile.Execute((args.ClickedItem) as PaperTileViewModel);
        }
    }
}
