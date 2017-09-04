namespace ImageGrabber.StatusUpdater
{
    /// <summary>
    /// Represents a class that updates status to the user
    /// </summary>
    public interface IStatusUpdater
    {
        /// <summary>
        /// Updates status by writing to console
        /// </summary>
        /// <param name="mangaName">The manga name</param>
        /// <param name="chapter">The chapter</param>
        /// <param name="page">The page</param>
        /// <param name="status">The status</param>
        void UpdateStatus(string mangaName, int chapter, int page, string status);
    }
}
