namespace ImageGrabber.Behaviors
{
    /// <summary>
    /// Represents an object that gets the URL of an image from a HTML response
    /// </summary>
    public interface IImageUrlFinder
    {
        /// <summary>
        /// Gets the Image URL
        /// </summary>
        /// <param name="htmlResponse">The HTML Body</param>
        /// <returns>The Image URL that we want to download</returns>
        string GetImageUrl(string htmlBody);
    }
}
