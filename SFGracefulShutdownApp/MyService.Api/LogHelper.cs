using System.Diagnostics;
using System.Fabric;

namespace MyService.Api
{
    public static class LogHelper
    {
        public static void Log(ServiceContext context, string message)
        {
            ServiceEventSource.Current.ServiceMessage(context, message);
            Trace.WriteLine(message);
        }
    }
}
