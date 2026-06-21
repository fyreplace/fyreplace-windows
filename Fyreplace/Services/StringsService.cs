using Microsoft.Windows.ApplicationModel.Resources;

namespace Fyreplace.Services
{
    public sealed class StringsService : IStringsService
    {
        private readonly ResourceLoader resources = new();

        public string GetString(string key) => resources.GetString(key);
    }
}
