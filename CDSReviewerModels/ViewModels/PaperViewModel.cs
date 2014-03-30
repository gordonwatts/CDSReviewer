
using Caliburn.Micro.Portable;
namespace CDSReviewerModels.ViewModels
{
    /// <summary>
    /// Display detailed infomration for a single paper, including access
    /// to all special things (like "open").
    /// </summary>
    public class PaperViewModel : ViewModelBase
    {
        public PaperViewModel(INavService nav)
            : base(nav)
        { }

        /// <summary>
        /// The ID of the paper we are to display. We use this as a direct
        /// key into the local database.
        /// </summary>
        public string PaperID { get; set; }
    }
}
