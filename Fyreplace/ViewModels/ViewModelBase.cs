using CommunityToolkit.Mvvm.ComponentModel;
using Fyreplace.Events;
using Fyreplace.Services;
using Sentry;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fyreplace.ViewModels
{
    public abstract partial class ViewModelBase : ObservableObject
    {
        protected IEventBus EventBus = AppBase.GetService<IEventBus>();

        protected async Task<T?> Call<T>(Func<Task<T>> action, Func<ApiException, FailureEvent?> onFailure)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException)
            {
                await EventBus.Publish(new FailureEvent("Error_Connection"));
            }
            catch (ApiException exception)
            {
                var failureEvent = onFailure(exception);

                if (failureEvent != null)
                {
                    await EventBus.Publish(failureEvent);
                }
            }
            catch (Exception exception)
            {
                await EventBus.Publish(new FailureEvent());
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
