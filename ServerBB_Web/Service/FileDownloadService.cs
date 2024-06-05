using ServerBB_Web.Service.Interface;
using Microsoft.JSInterop;

namespace ServerBB_Web.Service
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