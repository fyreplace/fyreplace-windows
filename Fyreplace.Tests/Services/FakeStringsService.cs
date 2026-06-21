using Fyreplace.Services;

namespace Fyreplace.Tests.Services
{
    public class FakeStringsService : IStringsService
    {
        public string GetString(string key) => key;
    }
}
