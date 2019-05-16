using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace MyService.Api
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class Api : StatelessService
    {
        public Api(StatelessServiceContext context)
            : base(context)
        {
            ServiceStatusLog.Instance.InflightRequestCount = 1;
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new MyHttpListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Http Sys on {url}");

                        return new WebHostBuilder()
                            .UseHttpSys(options => { options.Timeouts.IdleConnection = TimeSpan.FromMinutes(10); })
                            .ConfigureServices(
                                services => services
                                    .AddSingleton(serviceContext))
                            .UseContentRoot(Directory.GetCurrentDirectory())
                            .UseStartup<Startup>()
                            .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                            .UseUrls(url)
                            .Build();
                    }))
            };
        }

        protected override Task RunAsync(CancellationToken cancellationToken)
        {
            LogHelper.Log(Context, $"RunAsync Called => CancellationToken.IsCancellationRequested={cancellationToken.IsCancellationRequested}, Inflight Request Count={ServiceStatusLog.Instance.InflightRequestCount}");
            return base.RunAsync(cancellationToken);
        }

        protected override void OnAbort()
        {
            LogHelper.Log(Context, $"OnAbort Called => Inflight Request Count={ServiceStatusLog.Instance.InflightRequestCount}");
            base.OnAbort();
        }

        protected override async Task OnCloseAsync(CancellationToken cancellationToken)
        {
            LogHelper.Log(Context, $"OnCloseAsync Called => CancellationToken.IsCancellationRequested={cancellationToken.IsCancellationRequested}, Inflight Request Count={ServiceStatusLog.Instance.InflightRequestCount}");
            ServiceStatusLog.Instance.CancellationToken = cancellationToken;

            var sw = Stopwatch.StartNew();

            // Check for any inflight requests and only allow 30 seconds for them to finish processing otherwise, go ahead and close
            while (ServiceStatusLog.Instance.InflightRequestCount > 1 && sw.ElapsedMilliseconds < TimeSpan.FromSeconds(30).Milliseconds)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

            sw.Stop();

            ServiceStatusLog.Instance.InflightRequestCount = 0;

            //await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            await base.OnCloseAsync(cancellationToken);
        }

    }
}
