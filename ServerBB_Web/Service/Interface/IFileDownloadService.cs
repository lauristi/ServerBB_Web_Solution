namespace ServerBB_Web.Service.Interface
{
    public interface IFileDownloadService
    {
        Task DownloadFile(string url, string fileName);
    }
}