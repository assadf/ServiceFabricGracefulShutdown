using System.Threading;

namespace MyService.Api
{
    public sealed class ServiceStatusLog
    {
        public int InflightRequestCount;

        public CancellationToken CancellationToken { get; set; }

        public string ServiceName { get; set; }

        private ServiceStatusLog() {}

        public static ServiceStatusLog Instance { get; } = new ServiceStatusLog();
    }
}
