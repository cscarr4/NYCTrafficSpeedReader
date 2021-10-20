using FileHelpers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NYCTrafficSpeedReader
{
    class TrafficApplication : IDisposable
    {
        private const string sourceUrl = "http://207.251.86.229/nyc-links-cams/LinkSpeedQuery.txt";

        private readonly ILogger<TrafficApplication> logger;
        private readonly DataDownloader downloader;
        private readonly FileHelperEngine<TrafficDataRecord> engine;
        private readonly EventHubSupplier supplier;
        private readonly Timer processTimer;
        private readonly AppStartParameters parameters;

        public bool IsRunning { get; private set; }

        public TrafficApplication(ILogger<TrafficApplication> logger, DataDownloader downloader, FileHelperEngine<TrafficDataRecord> engine, EventHubSupplier supplier)
        {
            this.logger = logger;
            this.downloader = downloader;
            this.engine = engine;
            this.supplier = supplier;
            processTimer = new Timer(async (state) => await Process());
            parameters = GetParameters();
        }

        public void Dispose()
        {
            processTimer.Dispose();
        }

        public void Start()
        {
            if (parameters == null)
                return;

            processTimer.Change(0, 5000);
            IsRunning = true;
        }

        public void Stop()
        {
            processTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsRunning = false;
        }

        private async Task Process()
        {
            logger.LogInformation("Begin processing");

            var bytes = await downloader.GetDataAsync(parameters.SourceUrl);
            if (bytes == null || bytes.Length == 0)
            {
                logger.LogInformation("No data retrieved");
                return;
            }

            var content = Encoding.UTF8.GetString(bytes);
            var result = engine.ReadString(content);
            logger.LogInformation($"Retrieved {result.Length} total records");

            var boroughResult = result.Where(r => r.Borough == parameters.Borough).ToArray();
            logger.LogInformation($"{boroughResult.Count()} records for {parameters.Borough}");

            await supplier.SendData(parameters.ConnectionString, parameters.EventHubName, boroughResult);
            logger.LogInformation("Processing complete");
        }

        private AppStartParameters GetParameters()
        {
            var connectionString = Environment.GetEnvironmentVariable("connectionString");
            var eventHubName = Environment.GetEnvironmentVariable("eventHubName");
            var borough = Environment.GetEnvironmentVariable("borough");

            #region Validation
            var isSuccessful = true;
            var errorMessage = new StringBuilder("Error retrieving parameters:" + Environment.NewLine);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                isSuccessful = false;
                errorMessage.AppendLine("ConnectionString missing");
            }

            if (string.IsNullOrWhiteSpace(eventHubName))
            {
                isSuccessful = false;
                errorMessage.AppendLine("EventHubName missing");
            }

            if (string.IsNullOrWhiteSpace(borough))
            {
                isSuccessful = false;
                errorMessage.AppendLine("Borough missing");
            }

            if (!isSuccessful)
            {
                logger.LogError(errorMessage.ToString());
                return null;
            } 
            #endregion

            logger.LogInformation($"ConnectionString: {connectionString}\nEventHubName: {eventHubName}\nBorough: {borough}");

            return new AppStartParameters
            {
                Borough = borough,
                ConnectionString = connectionString,
                EventHubName = eventHubName,
                SourceUrl = sourceUrl
            };
        }
    }
}
