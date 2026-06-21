using Fyreplace.Data;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fyreplace.Services
{
    public sealed partial class TokenRefreshService(ISecrets secrets, IApiClient api) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromDays(1);

                try
                {
                    if (!string.IsNullOrEmpty(secrets.Token))
                    {
                        secrets.Token = await api.GetNewTokenAsync(stoppingToken);
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
