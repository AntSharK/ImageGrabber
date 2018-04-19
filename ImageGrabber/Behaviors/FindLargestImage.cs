using HtmlAgilityPack;
using System.Collections.Generic;

namespace ImageGrabber.Behaviors
{
    /// <summary>
    /// Takes the largest (size-size) element with the IMG tag
    /// </summary>
    public class FindLargestImage : IImageUrlFinder
    {
        /// <inheritdoc />
        public IEnumerable<string> GetImageUrls(string htmlBody)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlBody);

            var images = doc.DocumentNode.Descendants("img");

            string imageUrl = string.Empty;
            int maxArea = -1;
            foreach (var image in images)
            {
                var src = image.GetAttributeValue("src", string.Empty);
                var width = image.GetAttributeValue("width", 0);
                var height = image.GetAttributeValue("height", 0);
                var area = width * height;
                if (area > maxArea)
                {
                    imageUrl = src;
                    maxArea = area;
                }
            }

            yield return imageUrl;
        }
    }
}
