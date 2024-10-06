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
        protected readonly IPreferences preferences = AppBase.GetService<IPreferences>();
        protected readonly ISecrets secrets = AppBase.GetService<ISecrets>();
        protected readonly IEventBus eventBus = AppBase.GetService<IEventBus>();

        protected async Task<T?> CallAsync<T>(Func<Task<T>> action, Func<HttpStatusCode, ViolationReport?, ExplainedFailure?, FailureEvent?>? onFailure = null)
        {
            onFailure ??= (_, _, _) => new FailureEvent();
            FailureEvent? failureEvent;
            Exception? capturedException = default;

            try
            {
                return await action();
            }
            catch (HttpRequestException)
            {
                failureEvent = new FailureEvent("Error_Connection");
            }
            catch (ApiException exception)
            {
                var statusCode = (HttpStatusCode)exception.StatusCode;

                if (statusCode == HttpStatusCode.Unauthorized)
                {
                    secrets.Token = string.Empty;
                    failureEvent = new FailureEvent("Error_Unauthorized");
                }
                else
                {
                    var violationReport = (exception as ApiException<ViolationReport>)?.Result;
                    var explainedFailure = (exception as ApiException<ExplainedFailure>)?.Result;
                    failureEvent = onFailure(statusCode, violationReport, explainedFailure);
                    capturedException = exception;
                }
            }
            catch (Exception exception)
            {
                failureEvent = new FailureEvent();
                capturedException = exception;
            }

            if (failureEvent != null)
            {
                await eventBus.PublishAsync(failureEvent);

                if (capturedException != null && failureEvent.Key == new FailureEvent().Key)
                {
                    SentrySdk.CaptureException(capturedException);
                }
            }

            return default;
        }

        protected Task CallAsync(Func<Task> action, Func<HttpStatusCode, ViolationReport?, ExplainedFailure?, FailureEvent?>? onFailure = null) => CallAsync(async () =>
            {
                await action();
                return true;
            },
            onFailure
        );
    }
}
