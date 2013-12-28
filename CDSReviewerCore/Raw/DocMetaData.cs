
using System;
namespace CDSReviewerCore.Raw
{
    /// <summary>
    /// document meta data
    /// </summary>
    internal class DocMetaData : IDocumentMetadata
    {
        /// <summary>
        /// Get/Set the main title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Get/Set the URI to the main document file.
        /// </summary>
        public Uri MainDocument { get; set; }

        /// <summary>
        /// Get/Set the abstract for the doc.
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Get/Set the author list.
        /// </summary>
        public string[] Authors { get; set; }
    }
}
