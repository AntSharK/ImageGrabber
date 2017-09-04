using System;

namespace ImageGrabber
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Current usage: ImageDownloader.exe <Path-To-Configuration-File>, <Path-To-Write-To>, <Image-Section-Name>, <First-Chapter>, <Last-Chapter>");
                return;
            }

            var pathToConfigFile = args[0];
            var pathToWriteTo = args[1];
            var imageSectionName = args[2];

            var firstChapter = 1;
            var lastChapter = 999;

            if (args.Length == 5)
            {
                try
                {
                    firstChapter = int.Parse(args[3]);
                    lastChapter = int.Parse(args[4]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unable to parse first and last chapter parameters: {0}", ex);
                    return;
                }
            }

            var imageDownloader = ImageDownloader.ImageDownloader.Load(pathToWriteTo, pathToConfigFile);
            imageDownloader.DownloadImages(imageSectionName, firstChapter, lastChapter).Wait();
        }
    }
}
