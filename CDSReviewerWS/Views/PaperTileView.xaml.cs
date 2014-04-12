using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CDSReviewerWS.Views
{
    public sealed partial class PaperTileView : UserControl
    {
        public PaperTileView()
        {
            this.InitializeComponent();
            PaperTitle.MaxHeight = PaperTitle.FontSize * 2.0 / 72 * DisplayInformation.GetForCurrentView().LogicalDpi;
        }
    }
}
