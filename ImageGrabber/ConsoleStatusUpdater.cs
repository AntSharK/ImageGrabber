using System;

namespace ImageGrabber
{
    /// <summary>
    /// A class to update status
    /// Can be abstracted away like general logging, but not needed for now
    /// </summary>
    public class ConsoleStatusUpdater
    {
        private const string StatusFormat = "{0}::{1}.{2} - {3}";

        /// <summary>
        /// Updates status by writing to console
        /// </summary>
        /// <param name="mangaName">The manga name</param>
        /// <param name="chapter">The chapter</param>
        /// <param name="page">The page</param>
        /// <param name="status">The status</param>
        public void UpdateStatus(string mangaName, int chapter, int page, string status)
        {
            Console.WriteLine(string.Format(StatusFormat, mangaName, chapter, page, status));
        }
    }
}
