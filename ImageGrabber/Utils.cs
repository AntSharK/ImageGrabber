using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace ImageGrabber
{
    /// <summary>
    /// Utility methods
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Too lazy to use actual errors, so errors are represented by using a prefix in the response
        /// </summary>
        public const string ErrorPrefix = "ERROR_";

        /// <summary>
        /// Gets the response body
        /// In the event of exception, returns a string with an error prefix
        /// Note that exceptions are expected in the case of getting back a 404
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <returns>The response body</returns>
        public static async Task<string> GetResponseBody(string requestUrl)
        {
            var httpRequest = WebRequest.Create(requestUrl);
            httpRequest.Timeout = 5000;

            try
            {
                using (var response = await httpRequest.GetResponseAsync())
                {
                    var httpResponse = response as HttpWebResponse;
                    if (httpResponse == null)
                    {
                        return ErrorPrefix + "NOTHTTP";
                    }

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        return ErrorPrefix + "INVALIDRESPONSE_" + httpResponse.StatusCode;
                    }

                    using (var stream = new StreamReader(response.GetResponseStream()))
                    {
                        return await stream.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return ErrorPrefix + "FAILURE";
            }
        }

        /// <summary>
        /// Downloads a file
        /// Also creates the directory
        /// </summary>
        /// <param name="downloadUrl">The URL to download the file from</param>
        /// <param name="fileDirectory">The directory to save the file in locally</param>
        /// <param name="fileName">The name of the file to use</param>
        public static void DownloadFile(string downloadUrl, string fileDirectory, string fileName)
        {
            Directory.CreateDirectory(fileDirectory);
            using (var client = new WebClient())
            {
                client.DownloadFile(downloadUrl, fileDirectory + "\\" + fileName);
            }
        }

        /// <summary>
        /// Logs an exception message
        /// </summary>
        /// <param name="fileDirectory">The directory of the file</param>
        /// <param name="exception">The exception message to log</param>
        public static void LogException(string fileDirectory, string exception)
        {
            Directory.CreateDirectory(fileDirectory);
            using (var stream = File.AppendText(fileDirectory + "\\IGLOG.txt"))
            {
                stream.WriteLine(exception);
            }
        }
    }
}
