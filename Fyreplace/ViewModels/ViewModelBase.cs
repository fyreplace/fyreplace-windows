using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Data;
using Fyreplace.Events;
using Fyreplace.Services;
using Sentry;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        protected readonly ISecrets secrets = AppBase.GetService<ISecrets>();
        protected readonly IEventBus eventBus = AppBase.GetService<IEventBus>();

        protected async Task<T?> Call<T>(Func<Task<T>> action, Func<ApiException, FailureEvent?> onFailure)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException)
            {
                await eventBus.Publish(new FailureEvent("Error_Connection"));
            }
            catch (ApiException exception)
            {
                if (exception.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    secrets.Token = "";
                    await eventBus.Publish(new FailureEvent("Error_Unauthorized"));
                }
                else
                {
                    var failureEvent = onFailure(exception);

                    if (failureEvent != null)
                    {
                        await eventBus.Publish(failureEvent);
                    }
                }
            }
            catch (Exception exception)
            {
                await eventBus.Publish(new FailureEvent());
                SentrySdk.CaptureException(exception);
            }

            return default;
        }

        protected Task Call(Func<Task> action, Func<ApiException, FailureEvent?> onFailure) => Call(async () =>
            {
                await action();
                return true;
            },
            onFailure
        );
    }
}
