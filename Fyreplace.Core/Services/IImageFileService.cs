using System.Threading.Tasks;
using Windows.Storage;

namespace Fyreplace.Services
{
    public interface IImageFileService
    {
        public Task<StorageFile?> PickImageFileAsync();
    }
}
