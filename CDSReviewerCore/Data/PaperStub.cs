
namespace CDSReviewerCore.Data
{
    /// <summary>
    /// A paper stub. Meant to include the simplest stuff for a paper.
    /// </summary>
    public class PaperStub
    {
        /// <summary>
        /// A unique string that specifies this value.
        /// </summary>
        /// <remarks>
        /// Not used in UX.
        /// Deterministic (two computers would come up with the same ID for the same paper).
        /// Unique (no two papers have the same one).
        /// </remarks>
        public string ID { get; set; }

        /// <summary>
        /// The title that will be displayed to the user.
        /// </summary>
        public string Title { get; set; }
    }
}
