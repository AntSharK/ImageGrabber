namespace ImageGrabber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var minChapter = 7;
            var maxChapter = 8;
            var mangaName = "fairy-tail";
            var writeFolder = "D:\\Test";

            var mangaReader = new MangaReader(writeFolder);
            mangaReader.DownloadImages(mangaName, minChapter, maxChapter).Wait();
        }
    }
}
