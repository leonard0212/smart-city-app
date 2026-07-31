using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Exceptions;
using SmartCity.Domain.Models.Common;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.AzureStorage;
using SmartCity.Domain.ServiceModels.File;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services.Common;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;
using SmartCity.Interfaces.ValidatorsServices;
using System.Drawing;
using SmartCity.Domain.ServiceModels.AzureStorage;
using Azure.Core;

namespace SmartCity.Core.Services.Common
{
    public class FileService : ServiceBase, IFileService
    {

        private readonly IGenericRepositorySimpleUniqueIdentifier<AppFile> _fileRepository;


        private readonly IFileServiceValidator _fileServiceValidator;
        private readonly ISystemProperyService _systemProperyService;
        private readonly IAzureStorageService _azureStorageService;
        private readonly ICacheService _cacheService;
        private readonly string _applicationName = "SmartCityFileStorage";
        public FileService(
             IApplicationContext<User> applicationContext
            , IGenericRepositorySimpleUniqueIdentifier<AppFile> fileRepository
            , IFileServiceValidator fileServiceValidator
            , ISystemProperyService systemProperyService

            , IAzureStorageService azureStorageService
            , ICacheService cacheService
           ) : base(applicationContext)
        {
            _fileRepository = fileRepository;
            _fileServiceValidator = fileServiceValidator;
            _systemProperyService = systemProperyService;
            _azureStorageService = azureStorageService;
            _cacheService = cacheService;
        }

        public async Task<CreateFileOut> UploadFile(CreateFileIn request)
        {
            await _fileServiceValidator.ValidateUploadFileAsync(request);
            var saveFileToAzure = await UploadFileToAzure();
            var ext = Path.GetExtension(request.FileName);
            var fileName = Path.GetFileNameWithoutExtension(request.FileName);



            var fullName = $"{fileName}{Guid.NewGuid()}{ext}";
            var relativePath = $"{request.Category}/{DateTime.Now.ToString("ddMMyyyy")}/{fullName}";



            var filePath = saveFileToAzure ? "Azure" : await SaveFileLocal(request.File, relativePath);
            if (saveFileToAzure)
            {
                var requestAzure = new AzureUploadRequest
                {
                    ApplicationName = _applicationName,
                    Base64Content = Convert.ToBase64String(request.File),
                    FileName = $"{fullName}",
                    FolderName = $"{request.Category}/{DateTime.Now.ToString("ddMMyyyy")}",
                };
                await _azureStorageService.UploadFileAsync(requestAzure);
            }

            var file = new Domain.Models.Common.AppFile()
            {
                ContentType = request.ContentType,
                Path = filePath,
                Name = request.FileName,
                FullName = fullName,
                Extension = ext,
                Category = request.Category,
                User = SecurityContext.User
            };
            await _fileRepository.SaveOrUpdateAsync(file);
            await _fileRepository.CommitChangesAsync();

            return new CreateFileOut { Id = file.Id };
        }

        public async Task<GetFileOut> GetFile(GetFileIn request)
        {
            var file = await _fileRepository.GetAsync(request.FileId);

            byte[] fileBytes = null;
            if (file.Path == "Azure")
            {
                var stringContent = await _azureStorageService.DownloadFileAsync(new AzureDownloadRequest
                {
                    ApplicationName = _applicationName,
                    FileName = file.FullName,
                    FolderName = $"{file.Category}/{file.CreatedAt.ToString("ddMMyyyy")}"
                });
                fileBytes = Convert.FromBase64String(stringContent);
            }
            else
            {
                fileBytes = await GetBytesAsync(file.Path);
            }


            var resp = new GetFileOut();
            resp.ContentType = file.ContentType;
            resp.Category = file.Category;
            resp.Name = file.Name;
            resp.Extension = file.Extension;
            resp.Content = fileBytes;
            return resp;
        }

        public async Task<FileStreamResultModel> GetImageStream(Guid fileId, int? width = null)
        {
            var file = await _fileRepository.GetAsync(fileId);

            var stream = await _azureStorageService.OpenReadStreamAsync(new AzureDownloadRequest
            {
                ApplicationName = _applicationName,
                FileName = file.FullName,
                FolderName = $"{file.Category}/{file.CreatedAt:ddMMyyyy}"
            });

            if (width.HasValue)
            {
                stream = ResizeImageStream(stream, width.Value);
            }

            return new FileStreamResultModel
            {
                Stream = stream,
                ContentType = file.ContentType,
                FileName = file.Name,
                Extension = file.Extension,
                Category = file.Category
            };
        }
        private Stream ResizeImageStream(Stream inputStream, int maxWidth)
        {
            if (inputStream == null || !inputStream.CanRead)
                throw new InvalidOperationException("Stream is null or unreadable");

            using var ms = new MemoryStream();
            inputStream.CopyTo(ms);
            ms.Position = 0;

            using var originalImage = System.Drawing.Image.FromStream(ms);

            if (originalImage.Width == 0 || originalImage.Height == 0)
                throw new InvalidOperationException("Image has invalid dimensions.");

            double scale = Math.Min(1.0, (double)maxWidth / originalImage.Width);
            int newWidth = (int)(originalImage.Width * scale);
            int newHeight = (int)(originalImage.Height * scale);

            var resizedBitmap = new Bitmap(newWidth, newHeight);
            using (var g = Graphics.FromImage(resizedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            var outputStream = new MemoryStream();
            resizedBitmap.Save(outputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            outputStream.Position = 0;
            return outputStream;
        }


        private async Task<byte[]> GetBytesAsync(string path)
        {
            var filename = Path.Combine(HostingEnvironment.ContentRootPath, path);
            var fileBytes = System.IO.File.ReadAllBytes(filename);

            return await Task.FromResult(fileBytes);
        }

        public async Task<string> GetFileUrl(GetFileIn request)
        {
            var file = await _fileRepository.GetAsync(request.FileId);
            var sasUrl = await _azureStorageService.GetSecureBlobSasUrl(_applicationName, $"{file.Category}/{file.CreatedAt:ddMMyyyy}",
                file.FullName, TimeSpan.FromMinutes(30));

            return $"{sasUrl}";
        }

        private async Task<string> SaveFileLocal(byte[] file, string relativePath)
        {
            var storeFilesPath = await _systemProperyService.GetSystemProperyValue("StoreFilesPath");
            var filePath = Path.Combine(storeFilesPath, relativePath);
            var fileInfo = new FileInfo(filePath);

            if (!fileInfo.Exists)
                Directory.CreateDirectory(fileInfo.Directory.FullName);

            using (var stream = File.Create(filePath))
            {
                stream.Write(file, 0, file.Length);
            }

            return filePath;
        }

        private async Task<bool> UploadFileToAzure()
        {
            var _cacheStoreFilesToAzure = "_cacheStoreFilesToAzure";
            var savefile = _cacheService.GetValue<string>(_cacheStoreFilesToAzure);
            if (savefile != null)
                return savefile == "true";
            var storeFileAzure = await _systemProperyService.GetSystemProperyValue("StoreFilesToAzure");
            if (string.IsNullOrEmpty(storeFileAzure))
                throw new ValidationException("StoreFilesToAzure SystemPropery not set");

            _cacheService.SetData<string>(_cacheStoreFilesToAzure, storeFileAzure, 5);
            return storeFileAzure == "true";

        }


    }
}
