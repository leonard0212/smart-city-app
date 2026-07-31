using SmartCity.Domain.ServiceModels.AzureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services.Common
{
    public interface IAzureStorageService
    {
        Task UploadFileAsync(AzureUploadRequest uploadModel);
        Task<string> DownloadFileAsync(AzureDownloadRequest downloadModel);
        Task<Stream> OpenReadStreamAsync(AzureDownloadRequest downloadModel);
        Task DeleteFileAsync(AzureDownloadRequest downloadModel);
        Task<string> GetSecureBlobSasUrl(string applicationName, string folderName, string fileName, TimeSpan validFor);
    }
}
