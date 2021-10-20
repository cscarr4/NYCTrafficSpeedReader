using System;
using System.Net;
using FileHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NYCTrafficSpeedReader
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var app = serviceProvider.GetService<TrafficApplication>();            

            try
            {
                app.Start();

                while (app.IsRunning)
                {

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                app.Stop();
            }

            Console.ReadLine();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole());
            services.AddTransient<DataDownloader>();
            services.AddTransient<TrafficApplication>();
            services.AddTransient<FileHelperEngine<TrafficDataRecord>>();
            services.AddTransient<EventHubSupplier>();
        }
    }
}
