
namespace CDSReviewerCore.Services
{
    /// <summary>
    /// An interface to help with dealing with file OS interfaces.
    /// For example, to open a file.
    /// </summary>
    public interface IOSFileHandler
    {
        /// <summary>
        /// We have a file stored locally. This will get it opened
        /// by the base OS.
        /// </summary>
        bool OpenFile();
    }
}
