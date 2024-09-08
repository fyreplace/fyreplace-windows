using Fyreplace.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Fyreplace.Tests.Services
{
    public sealed class FakeApiException(HttpStatusCode status) : ApiException(
        status.ToString(),
        (int)status,
        string.Empty,
        new ReadOnlyDictionary<string, IEnumerable<string>>(
            new Dictionary<string, IEnumerable<string>>()
            {
                ["Content-Type"] = ["text/plain"]
            }
        ),
        null
    )
    {
    }
}
