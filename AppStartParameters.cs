using System;
using System.Collections.Generic;
using System.Text;

namespace NYCTrafficSpeedReader
{
    class AppStartParameters
    {
        public string SourceUrl { get; set; }
        public string Borough { get; set; }
        public string ConnectionString { get; set; }
        public string EventHubName { get; set; }
    }
}
