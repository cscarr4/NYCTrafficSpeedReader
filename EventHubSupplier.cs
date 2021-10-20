using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace NYCTrafficSpeedReader
{
    class EventHubSupplier
    {
        private readonly ILogger<EventHubSupplier> logger;

        public EventHubSupplier(ILogger<EventHubSupplier> logger)
        {
            this.logger = logger;
        }

        public async Task SendData(string connectionString, string eventHubName, TrafficDataRecord[] records)
        {
            logger.LogInformation($"Sending {records.Length} records to {eventHubName}");

            var client = new EventHubProducerClient(connectionString, eventHubName);

            using (var eventBatch = await client.CreateBatchAsync())
            {
                try
                {
                    foreach (var record in records)
                    {
                        var json = JsonSerializer.Serialize(record);
                        var eventData = new EventData(json);
                        eventBatch.TryAdd(eventData);
                    }

                    await client.SendAsync(eventBatch);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Unexpected error sending data to {eventHubName}");
                }
                finally
                {
                    await client.DisposeAsync();
                }
                
            }

            logger.LogInformation($"Records sent to {eventHubName}");
        }
    }
}
