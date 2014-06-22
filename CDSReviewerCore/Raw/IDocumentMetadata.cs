
namespace CDSReviewerCore.Raw
{
    internal interface IDocumentMetadata
    {
        /// <summary>
        /// The title of the document
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Returns the abstract
        /// </summary>
        string Abstract { get; }

        /// <summary>
        /// Returns all the authors.
        /// </summary>
        string[] Authors { get; }
    }
}
