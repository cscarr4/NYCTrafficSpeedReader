using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NYCTrafficSpeedReader
{
    class DataDownloader
    {
        private readonly ILogger<DataDownloader> logger;

        public DataDownloader(ILogger<DataDownloader> logger)
        {
            this.logger = logger;
        }

        public async Task<byte[]> GetDataAsync(string sourceUri)
        {
            logger.LogInformation($"Downloading data from {sourceUri}");

            using (var client = new WebClient())
            {
                try
                {
                    var uri = new Uri(sourceUri);
                    var bytes = await client.DownloadDataTaskAsync(uri);
                    logger.LogInformation($"Successfully downloaded {bytes.Length} bytes");

                    return bytes;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An unexpected error occurred while trying to download data from {sourceUri}");
                    return null;
                }
            }
        }
    }
}
