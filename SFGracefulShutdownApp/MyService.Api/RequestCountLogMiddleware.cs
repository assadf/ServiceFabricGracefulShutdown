using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace MyService.Api
{
    public static class RequestCountLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestCountLogMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestCountLogMiddleware>();
        }
    }

    public class RequestCountLogMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCountLogMiddleware(RequestDelegate next, IActionDescriptorCollectionProvider actionProvider)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, StatelessServiceContext serviceContext)
        {
            Interlocked.Increment(ref ServiceStatusLog.Instance.InflightRequestCount);
            LogHelper.Log(serviceContext, $"Inflight request count incremented to {ServiceStatusLog.Instance.InflightRequestCount}");

            await _next.Invoke(context).ConfigureAwait(false);

            Interlocked.Decrement(ref ServiceStatusLog.Instance.InflightRequestCount);
            LogHelper.Log(serviceContext, $"Inflight request count decremented to {ServiceStatusLog.Instance.InflightRequestCount}");
        }
    }
}
