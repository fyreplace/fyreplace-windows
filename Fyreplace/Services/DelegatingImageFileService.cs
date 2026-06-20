using Fyreplace.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Fyreplace.Services
{
    public sealed class DelegatingImageFileService(IServiceProvider provider) : IImageFileService
    {
        public Task<StorageFile?> PickImageFileAsync() => provider.GetRequiredService<MainWindow>().PickImageFileAsync();
    }
}
