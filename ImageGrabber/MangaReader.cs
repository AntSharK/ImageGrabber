using System;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ImageGrabber
{
    /// <summary>
    /// Represents an interaction with MangaReader.net
    /// This can be abstracted out to represent interactions with other sites... but for now I don't bother
    /// </summary>
    public class MangaReader
    {
        private const string ImageFormat = "http://www.mangareader.net/{0}/{1}/{2}";
        private ConsoleStatusUpdater statusUpdater = new ConsoleStatusUpdater();
        private string writeFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="MangaReader"/> class
        /// </summary>
        /// <param name="writeFolder">The folder to save images to</param>
        public MangaReader(string writeFolder)
        {
            this.writeFolder = writeFolder;
        }

        /// <summary>
        /// Downloads all pages
        /// </summary>
        /// <param name="mangaName">The manga name</param>
        /// <param name="minChapter">The min chapter</param>
        /// <param name="maxChapter">The max chapter</param>
        /// <returns>A task</returns>
        public async Task DownloadImages(string mangaName, int minChapter, int maxChapter)
        {
            var currentChapter = minChapter;
            var chapterIsValid = true;

            while (chapterIsValid && currentChapter <= maxChapter)
            {
                var fileDirectory = this.writeFolder + "\\" + mangaName + "\\" + currentChapter;
                var currentPage = 1;
                var pageIsValid = true;

                while (pageIsValid && currentPage <= 999)
                {
                    var requestUrl = string.Format(ImageFormat, mangaName, currentChapter, currentPage);
                    var response = await Utils.GetResponseBody(requestUrl);

                    if (response.StartsWith(Utils.ErrorPrefix))
                    {
                        this.statusUpdater.UpdateStatus(mangaName, currentChapter, currentPage, response);
                        pageIsValid = false;
                    }
                    else
                    {
                        try
                        {
                            var imageUrl = this.GetImageUrl(response);
                            var fileExtension = Path.GetExtension(imageUrl);
                            this.statusUpdater.UpdateStatus(mangaName, currentChapter, currentPage, "DOWNLOADING.");
                            Utils.DownloadFile(imageUrl, fileDirectory, currentPage + fileExtension);
                            currentPage++;
                            this.statusUpdater.UpdateStatus(mangaName, currentChapter, currentPage, "FINISHED DOWNLOADING.");
                        }
                        catch (Exception ex)
                        {
                            Utils.LogException(fileDirectory, "PAGE " + currentPage + " --- " + ex.ToString());
                            pageIsValid = false;
                            this.statusUpdater.UpdateStatus(mangaName, currentChapter, currentPage, "ERROR GETTING IMAGE.");
                        }
                    }
                }

                if (currentPage <= 1)
                {
                    chapterIsValid = false;
                    this.statusUpdater.UpdateStatus(mangaName, currentChapter, currentPage, "**ERROR ON FIRST PAGE**");
                }
                else
                {
                    currentChapter++;
                    this.statusUpdater.UpdateStatus(mangaName, currentChapter, currentPage, "MOVING TO NEXT CHAPTER.");
                }
            }
        }

        /// <summary>
        /// Gets the URL of the image to download in a page
        /// Takes the largest (size-wise) element with the IMG tag
        /// </summary>
        /// <param name="response">The HTML response</param>
        /// <returns>A string representing the URL of the image</returns>
        private string GetImageUrl(string response)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(response);

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

            return imageUrl;
        }
    }
}
