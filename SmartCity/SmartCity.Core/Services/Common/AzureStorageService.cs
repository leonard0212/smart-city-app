using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.AzureStorage;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services.Common;

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace SmartCity.Core.Services.Common
{
    public class AzureStorageService : ServiceBase, IAzureStorageService
    {
        private readonly ISystemProperyService _systemProperyService;
        private readonly ICacheService _cacheService;
        private readonly string _azureStorageConnectionStringTokenKey = "_azureStorageConnectionStringTokenKey";

        public AzureStorageService(IApplicationContext<User> applicationContext
            , ISystemProperyService systemProperyService
            , ICacheService cacheService) : base(applicationContext)
        {
            _systemProperyService = systemProperyService;
            _cacheService = cacheService;
        }

        public async Task UploadFileAsync(AzureUploadRequest uploadModel)
        {
            var containerClient = await GetBlobContainerClient(uploadModel.ApplicationName);
            if (!containerClient.Exists())
                await containerClient.CreateIfNotExistsAsync();

            var pathToUpload = GenerateFilePath(uploadModel.FolderName, uploadModel.FileName);
            var blobClient = containerClient.GetBlobClient(pathToUpload);
            if (blobClient.Exists())
                throw new ValidationException(@$"Blob {uploadModel.FileName} already exists.");

            var file = Convert.FromBase64String(uploadModel.Base64Content);
            using (var ms = new MemoryStream(file, false))
            {
                await blobClient.UploadAsync(ms);
            }
        }
        public async Task<string> DownloadFileAsync(AzureDownloadRequest downloadModel)
        {
            var containerClient = await GetBlobContainerClient(downloadModel.ApplicationName);
            if (!containerClient.Exists())
                throw new ValidationException(@$"No container found.");
            var pathToDownload = GenerateFilePath(downloadModel.FolderName, downloadModel.FileName);
            var blobClient = containerClient.GetBlobClient(pathToDownload);
            if (!blobClient.Exists())
                throw new ValidationException(@$"Blob {downloadModel.FileName} does not exists.");

            using var ms = new MemoryStream();
            await blobClient.DownloadToAsync(ms);
            var file = ms.ToArray();
            var filestring = Convert.ToBase64String(file);
            return filestring;
        }

        public async Task DeleteFileAsync(AzureDownloadRequest downloadModel)
        {
            var containerClient = await GetBlobContainerClient(downloadModel.ApplicationName);
            if (!containerClient.Exists())
                throw new ValidationException(@$"No container found.");
            var pathToDelete = GenerateFilePath(downloadModel.FolderName, downloadModel.FileName);
            var blobClient = containerClient.GetBlobClient(pathToDelete);
            if (!blobClient.Exists())
                throw new ValidationException(@$"Blob {downloadModel.FileName} does not exists.");
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<Stream> OpenReadStreamAsync(AzureDownloadRequest downloadModel)
        {
            var containerClient = await GetBlobContainerClient(downloadModel.ApplicationName);
            if (!await containerClient.ExistsAsync())
                throw new ValidationException($"No container found.");

            var pathToDownload = GenerateFilePath(downloadModel.FolderName, downloadModel.FileName);
            var blobClient = containerClient.GetBlobClient(pathToDownload);

            if (!await blobClient.ExistsAsync())
                throw new ValidationException($"Blob {downloadModel.FileName} does not exist.");

            return await blobClient.OpenReadAsync(); // Stream direct din blob
        }

        public async Task<string> GetSecureBlobSasUrl(string applicationName, string folderName, string fileName, TimeSpan validFor)
        {
            var containerClient = await GetBlobContainerClient(applicationName);
            var blobClient = containerClient.GetBlobClient($"{folderName}/{fileName}");

            if (!await blobClient.ExistsAsync())
                throw new FileNotFoundException($"Blob {fileName} not found in {applicationName}/{folderName}");

            var connectionString = await GetAzureStorageConnectionString();
            var blobUri = blobClient.Uri;

            var connBuilder = new BlobServiceClient(connectionString);
            var accountName = connBuilder.AccountName;

            var match = Regex.Match(connectionString, @"AccountKey=(.+?);");
            if (!match.Success)
                throw new InvalidOperationException("Unable to extract account key from connection string.");

            var accountKey = match.Groups[1].Value;

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = $"{folderName}/{fileName}",
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.Add(validFor)
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var credential = new StorageSharedKeyCredential(accountName, accountKey);
            var sasToken = sasBuilder.ToSasQueryParameters(credential).ToString();

            return $"{blobUri}?{sasToken}";
        }

        #region Hellpers
        private async Task<BlobContainerClient> GetBlobContainerClient(string applicationName)
        {
            var connectionString = await GetAzureStorageConnectionString();
            var blobServiceClient = new BlobServiceClient(connectionString);
            return blobServiceClient.GetBlobContainerClient(applicationName.ToLower());
        }

        private async Task<string> GetAzureStorageConnectionString()
        {
            var connstring = _cacheService.GetValue<string>(_azureStorageConnectionStringTokenKey);
            if (connstring != null)
                return connstring;

            var connectionString = await _systemProperyService.GetSystemProperyValue("AzureStorageConnectionString");
            if (string.IsNullOrEmpty(connectionString))
                throw new ValidationException("Azure Storage Connection String is not configured.");

            _cacheService.SetData<string>(_azureStorageConnectionStringTokenKey, connectionString, 60);
            return connectionString;
        }
        private string GenerateFilePath(string folderName, string fileName)
        {
            return $"{folderName}/{fileName}";
        }
        #endregion




    }
}
