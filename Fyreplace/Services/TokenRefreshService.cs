using Fyreplace.Data;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fyreplace.Services
{
    sealed class TokenRefreshService : BackgroundService
    {
        private readonly ISecrets secrets = AppBase.GetService<ISecrets>();

        private static IApiClient Api => AppBase.GetService<IApiClient>();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromDays(1);

                try
                {
                    if (!string.IsNullOrEmpty(secrets.Token))
                    {
                        secrets.Token = await Api.GetNewTokenAsync(stoppingToken);
                    }
                }
                catch
                {
                    delay = TimeSpan.FromHours(1);
                }
                finally
                {
                    await Task.Delay(delay, stoppingToken);
                }
            }
        }
    }
}
