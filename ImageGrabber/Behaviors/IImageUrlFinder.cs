using System.Collections.Generic;

namespace ImageGrabber.Behaviors
{
    /// <summary>
    /// Represents an object that gets the URL of an image from a HTML response
    /// </summary>
    public interface IImageUrlFinder
    {
        /// <summary>
        /// Gets the Image URLs for all images to download
        /// </summary>
        /// <param name="htmlResponse">The HTML Body</param>
        /// <returns>The Image URLs for all images that we want to download</returns>
        IEnumerable<string> GetImageUrls(string htmlBody);
    }
}
