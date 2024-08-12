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

        protected abstract IEvent? Handle(ApiException exception);

        protected async Task<T?> Call<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (HttpRequestException)
            {
                await EventBus.Publish(new FailureEvent("Error_Connection_Title", "Error_Connection_Message"));
            }
            catch (ApiException exception)
            {
                var e = Handle(exception);

                if (e != null)
                {
                    await EventBus.Publish(e);
                }
            }
            catch (Exception exception)
            {
                await EventBus.Publish(new FailureEvent());
                SentrySdk.CaptureException(exception);
            }

            return default;
        }

        protected Task Call(Func<Task> action) => Call(async () =>
        {
            await action();
            return true;
        });
    }
}
