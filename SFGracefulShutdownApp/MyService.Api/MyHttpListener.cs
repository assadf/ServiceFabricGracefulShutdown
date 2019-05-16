using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;

namespace MyService.Api
{
    public class MyHttpListener : HttpSysCommunicationListener
    {
        private readonly ServiceContext _serviceContext;

        public MyHttpListener(ServiceContext serviceContext, string endpointName, Func<string, AspNetCoreCommunicationListener, IWebHost> build) : base(serviceContext, endpointName, build)
        {
            _serviceContext = serviceContext;
        }

        public override async Task CloseAsync(CancellationToken cancellationToken)
        {
            try
            {
                LogHelper.Log(_serviceContext, $"Attempting to Close, CancellationToken.IsCancellationRequested={cancellationToken.IsCancellationRequested}");
                await base.CloseAsync(cancellationToken);
            }
            catch (Exception e)
            {
                LogHelper.Log(_serviceContext, $"Close Async Error {e}");
            }
        }
    }
}
