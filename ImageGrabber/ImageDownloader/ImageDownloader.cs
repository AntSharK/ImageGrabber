using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ImageGrabber.Behaviors;
using ImageGrabber.StatusUpdater;

namespace ImageGrabber.ImageDownloader
{
    /// <summary>
    /// The image downloader
    /// </summary>
    [Serializable()]
    public class ImageDownloader
    {
        /// <summary>
        /// The URL Format
        /// </summary>
        [XmlElement("UrlFormat")]
        public string UrlFormat;

        /// <summary>
        /// The Image Sections
        /// </summary>
        [XmlArray("ImageSections")]
        [XmlArrayItem("ImageSection", typeof(ImageSection))]
        public ImageSection[] ImageSections;

        /// <summary>
        /// The criteria used to select the images
        /// </summary>
        [XmlElement("SelectionCriteria")]
        public string SelectionCriteria;

        /// <summary>
        /// The folder to write to
        /// </summary>
        private string writeFolder;

        /// <summary>
        /// The status updater
        /// </summary>
        private IStatusUpdater statusUpdater = new ConsoleStatusUpdater();

        /// <summary>
        /// The image URL finder
        /// </summary>
        private IImageUrlFinder imageUrlFinder;

        /// <summary>
        /// Creates an ImageDownloader object by loading from a configuration file
        /// </summary>
        /// <param name="writeFolder">The folder to write responses to</param>
        /// <param name="configurationFilePath">The path to the configuration file</param>
        /// <returns>A new image downloaded</returns>
        public static ImageDownloader Load(string writeFolder, string configurationFilePath)
        {
            var serializer = new XmlSerializer(typeof(ImageDownloader));
            {
                using (var streamReader = new StreamReader(configurationFilePath))
                {
                    var imageDownloader = serializer.Deserialize(streamReader) as ImageDownloader;
                    imageDownloader.writeFolder = writeFolder;
                    imageDownloader.SetImageUrlFinder();

                    return imageDownloader;
                }
            }
        }

        /// <summary>
        /// Initiailizes a new instance of the <see cref="ImageDownloader"/> class
        /// </summary>
        private ImageDownloader()
        {
        }

        /// <summary>
        /// Sets the Image URL Finder
        /// </summary>
        public void SetImageUrlFinder()
        {
            switch (this.SelectionCriteria)
            {
                case "FindNumberedImages":
                    this.imageUrlFinder = new FindNumberedImages();
                    break;
                case "FindLargestImage":
                default:
                    this.imageUrlFinder = new FindLargestImage();
                    break;
            }
        }

        /// <summary>
        /// Downloads all pages
        /// </summary>
        /// <param name="mangaName">The manga name</param>
        /// <param name="minChapter">The min chapter</param>
        /// <param name="maxChapter">The max chapter</param>
        /// <returns>A task</returns>
        public async Task DownloadImages(string imageSectionName, int minChapter, int maxChapter)
        {
            var imageSection = this.ImageSections.FirstOrDefault((sectionToFind) => sectionToFind.Name.Equals(imageSectionName));
            string properName = imageSectionName;
            string urlName = imageSectionName;
            if (imageSection != null)
            {
                urlName = imageSection.UrlName;
                if (maxChapter > imageSection.LastSubsection)
                {
                    maxChapter = imageSection.LastSubsection;
                }
            }

            var currentChapter = minChapter;
            var chapterIsValid = true;

            while (chapterIsValid && currentChapter <= maxChapter)
            {
                var fileDirectory = this.writeFolder + "\\" + properName + "\\" + currentChapter.ToString().PadLeft(4, '0');
                var currentPage = 1;
                var pageIsValid = true;

                while (pageIsValid && currentPage <= 999)
                {
                    var requestUrl = string.Format(this.UrlFormat, urlName, currentChapter, currentPage);
                    var response = await Utils.GetResponseBody(requestUrl);

                    if (response.StartsWith(Utils.ErrorPrefix))
                    {
                        this.statusUpdater.UpdateStatus(properName, currentChapter, currentPage, response);
                        pageIsValid = false;
                    }
                    else
                    {
                        try
                        {
                            foreach (var imageUrl in this.imageUrlFinder.GetImageUrls(response))
                            {
                                var fileExtension = Path.GetExtension(imageUrl);
                                this.statusUpdater.UpdateStatus(properName, currentChapter, currentPage, "DOWNLOADING.");
                                Utils.DownloadFile(imageUrl, fileDirectory, currentPage + fileExtension);
                                this.statusUpdater.UpdateStatus(properName, currentChapter, currentPage, "FINISHED DOWNLOADING.");
                                currentPage++;
                            }

                            // If the string is equal with or without the current page, then the current page parameter is not valid
                            if (string.Format(this.UrlFormat, urlName, currentChapter, currentPage) == string.Format(this.UrlFormat, urlName, currentChapter))
                            {
                                pageIsValid = false;
                                this.statusUpdater.UpdateStatus(properName, currentChapter, currentPage, "FINISHED DOWNLOADING PAGE.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Utils.LogException(fileDirectory, "PAGE " + currentPage + " --- " + ex.ToString());
                            pageIsValid = false;
                            this.statusUpdater.UpdateStatus(properName, currentChapter, currentPage, "ERROR GETTING IMAGE.");
                        }
                    }
                }

                if (currentPage <= 1)
                {
                    chapterIsValid = false;
                    this.statusUpdater.UpdateStatus(properName, currentChapter, currentPage, "**ERROR ON FIRST PAGE**");
                }
                else
                {
                    this.statusUpdater.UpdateStatus(properName, currentChapter, currentPage, "MOVING TO NEXT SECTION.");
                    currentChapter++;
                }
            }
        }
    }
}
