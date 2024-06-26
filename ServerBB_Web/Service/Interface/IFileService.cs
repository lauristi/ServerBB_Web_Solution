using Microsoft.AspNetCore.Components.Forms;

namespace ServerBB_Web.Service.Interface
{
    public interface IFileService
    {
        Task<bool> UploadFileAsync(IBrowserFile file, string endpoint);

        Task<bool> ProcessFileAsync(string endpoint);

        Task ClearInputFile(string inputFileId);

        Task DownloadFile(string url, string fileName);

        Task DownloadFileByteAsync(string fileName, byte[] fileBytes);
    }
}