using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CDSReviewerWS.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePageView : Page
    {
        public HomePageView()
        {
            this.InitializeComponent();
        }

        public void ForkItUp(object sender, ItemClickEventArgs args)
        {
            // How do I get this information back to the view model?
        }
    }
}
