using Microsoft.JSInterop;
using ServerBB_Web.Service.Interface;

namespace ServerBB_Web.Service.Refs
{
    public class FileDownloadService : IFileDownloadService
    {
        private readonly IJSRuntime _jsRuntime;

        public FileDownloadService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task DownloadFile(string url, string fileName)
        {
            await _jsRuntime.InvokeVoidAsync("downloadService.downloadFile", url, fileName);
        }
    }
}