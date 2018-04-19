using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageGrabber.Behaviors
{
    /// <summary>
    /// Finds all images that are just numbers, and returns them in numerical order
    /// </summary>
    public class FindNumberedImages : IImageUrlFinder
    {
        /// <inheritdoc />
        public IEnumerable<string> GetImageUrls(string htmlBody)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlBody);

            var images = doc.DocumentNode.Descendants("img");

            List<Tuple<string, int>> imageUrls = new List<Tuple<string, int>>();
            foreach (var image in images)
            {
                var src = image.GetAttributeValue("src", string.Empty);
                var imageClass = image.GetAttributeValue("class", string.Empty);

                if (imageClass == "img_content")
                {
                    try
                    {
                        var url = new Uri(src);
                        var imageFileName = url.Segments[url.Segments.Length - 1];
                        var imageName = Path.GetFileNameWithoutExtension(imageFileName);
                        int fileNameNumber;
                        if (Int32.TryParse(imageName, out fileNameNumber))
                        {
                            imageUrls.Add(Tuple.Create(src, fileNameNumber));
                        }
                    }
                    catch
                    {
                        // Do nothing
                    }
                }
            }

            imageUrls.OrderBy(i => i.Item2);
            foreach (var item in imageUrls)
            {
                yield return item.Item1;
            }
        }
    }
}
