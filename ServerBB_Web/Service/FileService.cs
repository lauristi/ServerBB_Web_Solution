using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using ServerBB_Web.Service.Interface;
using System;

public class FileService : IFileService
{
    private readonly HttpClient _httpClient;
    private readonly IJSRuntime _jsRuntime;

    public FileService(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _httpClient = httpClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> UploadFileAsync(IBrowserFile file, string endpoint)
    {
        using (var memoryStream = new MemoryStream())
        {
            await file.OpenReadStream().CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(memoryStream), "file", file.Name);

            var response = await _httpClient.PostAsync(endpoint, content);

            return response.IsSuccessStatusCode;
        }
    }

    public async Task<bool> ProcessFileAsync(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            if (response.IsSuccessStatusCode)
            {
                var contentDisposition = response.Content.Headers.ContentDisposition;
                var fileName = contentDisposition.FileName;
                var url = response.RequestMessage.RequestUri.AbsoluteUri;

                await _jsRuntime.InvokeVoidAsync("downloadFile", url, fileName);
                return true;
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ocorreu um erro ao tentar baixar o arquivo.", ex);
        }
        return false;
    }

    public async Task ClearInputFile(string inputFileId)
    {
        await _jsRuntime.InvokeVoidAsync("clearInputFile", inputFileId);
    }

    public async Task DownloadFile(string url, string fileName)
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("downloadService.downloadFile", url, fileName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ocorreu um erro ao tentar baixar o arquivo.", ex);
        }
    }

    public async Task DownloadFileByteAsync(string fileName, byte[] fileBytes)
    {
        try
        {
            // Converte os bytes do arquivo para uma string base64
            string base64String = Convert.ToBase64String(fileBytes);

            // Cria um URI para o download do arquivo
            var uri = new Uri($"data:application/octet-stream;base64,{base64String}");

            // Inicia o download do arquivo chamando a função JavaScript
            await _jsRuntime.InvokeVoidAsync("downloadService.downloadFile", uri.AbsoluteUri, fileName);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Ocorreu um erro ao tentar baixar o arquivo.", ex);
        }



    }

}