using Fyreplace.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Text.Json.Nodes;

namespace Fyreplace.Tests.Services
{
    public sealed class FakeApiException(HttpStatusCode status, JsonObject? body = null) : ApiException(
        status.ToString(),
        (int)status,
        body?.ToString() ?? "",
        new ReadOnlyDictionary<string, IEnumerable<string>>(
            new Dictionary<string, IEnumerable<string>>()
            {
                ["Content-Type"] = [body != null ? "application/json" : "text/plain"]
            }
        ),
        null
    )
    {
    }
}
