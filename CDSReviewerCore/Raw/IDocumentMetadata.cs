
using System;
namespace CDSReviewerCore.Raw
{
    internal interface IDocumentMetadata
    {
        /// <summary>
        /// The title of the document
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The link to the main documment (most recent version of the document, etc.).
        /// </summary>
        Uri MainDocument { get; }

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
