
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
    }
}
