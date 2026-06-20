using Fyreplace.Services;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fyreplace.Tests.Services
{
    public class FakeImageFileService : IImageFileService
    {
        public Task<StorageFile?> PickImageFileAsync() => Task.FromResult<StorageFile?>(null);
    }
}
