using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartCity.Core.Extensions;
using SmartCity.Core.Utils;
using SmartCity.Domain;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ServiceModels.PotholeDetection;
using SmartCity.Domain.ServiceModels.RawDetection;
using SmartCity.Domain.ServiceModels.TraficSign;
using SmartCity.Domain.ServiceModels.Trash;
using SmartCity.Domain.Utils;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Common;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;

using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SmartCity.Core.Services
{
    public class RawDetectionService : ServiceBase, IRawDetectionService
    {
        private readonly IGenericRepositorySimpleUniqueIdentifier<RawDetection> _rawDetectionRepository;
        private readonly IFileService _fileService;
        private readonly IRoadInfrastructureService _roadInfrastructureService;
        private readonly ITrashService _trashService;
        private readonly IBillBoardService _billBoardService;
        private readonly IHttpClientService _httpClientService;
        private readonly ITrafficSignService _trafficSignService;
        private readonly IDictionary<string, Func<RawDetection, Task<int>>> _functionsGenerateDetections;


        public RawDetectionService(
            IApplicationContext<User> applicationContext
            , IGenericRepositorySimpleUniqueIdentifier<RawDetection> rawDetectionRepository
            , IFileService fileService
            , IRoadInfrastructureService roadInfrastructureService
            , IHttpClientService httpClientService
            , ITrashService trashService
            , IBillBoardService billBoardService
            , ITrafficSignService trafficSignService
            ) : base(applicationContext)
        {
            _rawDetectionRepository = rawDetectionRepository;
            _fileService = fileService;
            _roadInfrastructureService = roadInfrastructureService;
            _httpClientService = httpClientService;
            _trashService = trashService;
            _billBoardService = billBoardService;
            _trafficSignService = trafficSignService;


            _functionsGenerateDetections = new Dictionary<string, Func<RawDetection, Task<int>>>
            {
                { "Pothole", ImportRoadInfrastructure },
                { "Billboard", ImportBillBoardDetection },
                { "Trash", ImportTrashDetection },
                { "TrafficSign", ImportTrafficSignDetection }
            };



        }


        public async Task CreateRawDetectionAsync(string messageBusMessage, string messageId)
        {
            var request = JsonConvert.DeserializeObject<RawDetectionRequest>(messageBusMessage);

            var fileContent = await _httpClientService.GetImageBytesAsync(StringUtils.Coalesce(request.ImageUrlFull, request.ImageUrl));// Convert.FromBase64String(request.base64);
                                                                                                                                        // var fileContentPreview = await _httpClientService.GetImageBytesAsync(request.ImageUrlPreview);
            var cropFileContent = await _httpClientService.GetImageBytesAsync(request.ImageUrl);
            var previewFileContent = await _httpClientService.GetImageBytesAsync(request.ImageUrlFullPreview);


            var rawDetection = Mapper.Map<RawDetection>(request);
            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                var file = await _fileService.UploadFile(new CreateFileIn() { Category = "Detection", ContentType = "image/jpeg", File = fileContent, FileName = $"{Guid.NewGuid().ToString()}.jpg" });
                var cropFile = await _fileService.UploadFile(new CreateFileIn() { Category = "Detection", ContentType = "image/jpeg", File = cropFileContent, FileName = $"{Guid.NewGuid().ToString()}.jpg" });
                var previewFile = await _fileService.UploadFile(new CreateFileIn() { Category = "Detection", ContentType = "image/jpeg", File = previewFileContent, FileName = $"{Guid.NewGuid().ToString()}.jpg" });




                //trebuie luata imaginea din ImageUrl 960x960 care trebuie sa apara in tab 2 in interfata
                //puse coordonatele pe eavorbit cu ana daca le are
                //cu poza asta 960x960 trebuie apelat api2 ala care face crop si dupaia cu ala pus la AI in pas 2



                //  var previewFile = await _fileService.UploadFile(new Domain.ServiceModels.File.CreateFileIn() { Category = "Preview", ContentType = "image/jpeg", File = fileContentPreview, FileName = $"{Guid.NewGuid().ToString()}.jpg" });


                rawDetection.ServiceBusMessageId = messageId;
                rawDetection.FileId = file.Id;
                rawDetection.PreviewFileId = previewFile.Id;
                rawDetection.CropFileId = cropFile.Id;
                rawDetection.ProcessStatus = Domain.Models.Enums.RawDetectionProcessStatus.Unprocessed;

                await _rawDetectionRepository.SaveOrUpdateAsync(rawDetection);
                await _rawDetectionRepository.CommitChangesAsync();
            });


            //await ProcessPictureForBlurAsync(rawDetection.Id);
            await ProcessPictureAiAsync(rawDetection.Id);
            await ProcessRawDataInfoAiAsync(rawDetection.Id);


        }


        public async Task<List<RawDetection>> GetRawDetectionsListAsync()
        {
            var top = await _rawDetectionRepository.QueryAll()
                .Where(x => x.ProcessStatus == Domain.Models.Enums.RawDetectionProcessStatus.Unprocessed)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync();
            return top;
        }


        public async Task<(IPagedList<RawDetection> list, List<RawDetectionInfo> info, List<RawDetectionFile> previewFiles, List<RawDetectionFile> processedFiles)> GetIPagedListRawDetectionsListAsync(RawDetectionFilterServiceModel model)
        {
            model.PageIndex = model.PageIndex > 0 ? model.PageIndex : 1;
            model.PageSize = model.PageSize.HasValue && model.PageSize.Value > 0 ? model.PageSize.Value : 10;
            var where = QueryPredicateBuilder.True<RawDetection>();

            if (model.EdgeId.HasValue)
                where = where.And(d => d.EdgeId == model.EdgeId.Value);
            if (!string.IsNullOrEmpty(model.Category))
                where = where.And(d => d.MainClass == model.Category);
            if (!string.IsNullOrEmpty(model.SubCategory))
                where = where.And(d => d.SubClass == model.SubCategory);
            if (model.Status.HasValue)
                where = where.And(d => d.ProcessStatus == model.Status);

            // where = where.And(d => !d.IsDeleted.HasValue || !d.IsDeleted.Value);
            //if (model.Category != null)
            //    where = where.And(d => d.Category == model.Category);
            var order = QueryOrderBuilder.Create<RawDetection>(query => query.OrderByDescending(x => x.CreatedAt));
            var totalCount = await _rawDetectionRepository.CountAsync(where);
            var pagedList = await _rawDetectionRepository
            .GetPagedAsync(model.PageIndex.Value, model.PageSize.Value, where, order)
              .ToPagedListAsync(model.PageIndex.Value, model.PageSize.Value, totalCount, null);



            var infos = new List<RawDetectionInfo>();
            var previewFiles = new List<RawDetectionFile>();
            var processedFiles = new List<RawDetectionFile>();
            foreach (var item in pagedList)
            {

                try
                {
                    var info = await GetRawDetectionInfosAsync(item.Id);
                    infos.Add(new RawDetectionInfo { Id = item.Id, Infos = info });
                }
                catch (Exception ex)
                {

                    infos.Add(new RawDetectionInfo { Id = item.Id, Infos = null });
                }

                //try
                //{
                //    var previewFile = await GetRawDetectionPreviewFileAsync(item.Id);
                //    previewFiles.Add(new RawDetectionFile { Id = item.Id, File = previewFile });
                //}
                //catch (Exception ex)
                //{
                //    previewFiles.Add(new RawDetectionFile { Id = item.Id, File = null });
                //}

                //try
                //{
                //    var processedFile = await GetRawDetectionProcessedFileAsync(item.Id);
                //    processedFiles.Add(new RawDetectionFile { Id = item.Id, File = processedFile });
                //}
                //catch (Exception ex)
                //{
                //    processedFiles.Add(new RawDetectionFile { Id = item.Id, File = null });
                //}



            }





            return (pagedList, infos, previewFiles, processedFiles);
        }





        public async Task<Dictionary<string, string>> GetRawDetectionInfosAsync(Guid RawDetectionId)
        {
            var rawDetection = await _rawDetectionRepository.GetAsync(RawDetectionId);
            var result = rawDetection.ProcessedExtensionData;

            var dictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawDetection.ProcessedExtensionData);
            var dictonary = JsonConvert.DeserializeObject<Dictionary<string, string>>(dictionaryResult.FirstOrDefault().Value);

            return dictonary;
        }

        public async Task<string> GetRawDetectionProcessedFileAsync(Guid rawDetectionId, bool isPreview = false, bool isCrop = false)
        {
            var rawDetection = await _rawDetectionRepository.GetAsync(rawDetectionId);

            if (isCrop)
            {
                if (rawDetection.CropFileId == null)
                    throw new InvalidOperationException("Crop file does not exist for this raw detection.");

                return await _fileService.GetFileUrl(new GetFileIn { FileId = rawDetection.CropFileId.Value });
            }

            Guid? fileId = null;

            if (isPreview)
            {
                fileId = rawDetection.PreviewFileId;
            }
            if(!isCrop && !isPreview)
            {
                fileId = rawDetection.ProcessedsFileId;
            }

            if (fileId == null)
                throw new InvalidOperationException("Requested file does not exist for this raw detection.");

            //var file = await _fileService.GetFile(new Domain.ServiceModels.File.GetFileIn() { FileId = rawDetection.PreviewFileId.Value }, true);
            //var result = Mapper.Map<FileServiceModel>(file);
            //result.Content = ResizeImageKeepAspectRatio(result.Content, 1024, 768); // Resize the image to a maximum of 800x600 while keeping aspect ratio

            return await _fileService.GetFileUrl(new GetFileIn { FileId = fileId.Value });
        }

        public async Task<FileStreamResultModel> GetRawDetectionProcessedStreamImageAsync(Guid rawDetectionId, int? width = null)
        {
            var rawDetection = await _rawDetectionRepository.GetAsync(rawDetectionId);

            var streamResult = await _fileService.GetImageStream(rawDetection.ProcessedsFileId.Value, width);

            return streamResult; // DIRECT STREAM
        }

        public async Task<FileServiceModel> GetRawDetectionPreviewFileAsync(Guid rawDetectionId)
        {
            var rawDetection = await _rawDetectionRepository.GetAsync(rawDetectionId);
            var file = await _fileService.GetFile(new Domain.ServiceModels.File.GetFileIn() { FileId = rawDetection.PreviewFileId.Value });
            var result = Mapper.Map<FileServiceModel>(file);
            return result;
        }

        public async Task<FileServiceModel> GetRawDetectionFileAsync(Guid fileId)
        {
            var file = await _fileService.GetFile(new Domain.ServiceModels.File.GetFileIn() { FileId = fileId });
            var result = Mapper.Map<FileServiceModel>(file);
            return result;
        }
        public async Task DeleteRawDataAsync(Guid rawDetectionId)
        {
            var rawdata = await _rawDetectionRepository.GetAsync(rawDetectionId);
            rawdata.ProcessStatus = Domain.Models.Enums.RawDetectionProcessStatus.Deleted;
            await _rawDetectionRepository.SaveOrUpdateAsync(rawdata);
            await _rawDetectionRepository.CommitChangesAsync();

        }

        public async Task DeleteRawDataBatchAsync(IEnumerable<Guid> rawDetectionIds)
        {
            foreach (var id in rawDetectionIds)
            {
                var rawdata = await _rawDetectionRepository.GetAsync(id);
                if (rawdata == null) continue;
                rawdata.ProcessStatus = Domain.Models.Enums.RawDetectionProcessStatus.Deleted;
                await _rawDetectionRepository.SaveOrUpdateAsync(rawdata);
            }
            await _rawDetectionRepository.CommitChangesAsync();
        }

        public async Task ProcessRawDetection(Guid rawDetectionId)
        {
            var rawDetection = await _rawDetectionRepository.GetAsync(rawDetectionId);

            await ImportDetection(rawDetection);

            rawDetection.ProcessStatus = Domain.Models.Enums.RawDetectionProcessStatus.Processed;
            await _rawDetectionRepository.SaveOrUpdateAsync(rawDetection);
            await _rawDetectionRepository.CommitChangesAsync();

        }




        #region Hellpers
        private async Task ProcessPictureAiAsync(Guid rawDetectionId)
        {
            var rawDetection = await _rawDetectionRepository.QueryAll().Include(x => x.Detections).FirstOrDefaultAsync(x => x.Id == rawDetectionId);
            var file = await GetRawDetectionFileAsync(rawDetection.FileId.Value);
            var cropFile = await GetRawDetectionFileAsync(rawDetection.CropFileId.Value);

            var settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };


            var detectionsJson = JsonConvert.SerializeObject(rawDetection.Detections, settings);
            var detectionsCropJson = "";
            if (rawDetection.DetectionsCrop != null)
                detectionsCropJson = JsonConvert.SerializeObject(rawDetection.DetectionsCrop, settings);


            var result = await _httpClientService.ProcessAiWithFormData("http://10.254.42.5:8884/v1/adddetectionstoimage", file.Content, detectionsJson);
            var cropResult = await _httpClientService.ProcessAiWithFormData("http://10.254.42.5:8884/v1/adddetectionstoimage", cropFile.Content, detectionsCropJson);


            //File.WriteAllBytes("C:\\temp\\New folder\\result.png", result);
            //File.WriteAllBytes("C:\\temp\\New folder\\previewResult.png", cropResult);


            //var api2 = await _httpClientService.ProcessAiWithFormDataStringResult("http://10.254.42.5:8884/v1/process-image", result, detectionsJson);

            ////un tab full si una crop
            ////un cop la AI

            //var dicresult = JsonConvert.DeserializeObject<Dictionary<string, string>>(api2);

            //var previewUrl = dicresult["blob_url_preview"];
            //var cropUrl = dicresult["blob_url_crop"];


            var processedFile = await _fileService.UploadFile(new Domain.ServiceModels.File.CreateFileIn() { Category = "ProcessedDetection", ContentType = "image/jpeg", File = result, FileName = $"{Guid.NewGuid().ToString()}.jpg" });
            rawDetection.ProcessedsFileId = processedFile.Id;


            var cropFileeeee = await _fileService.UploadFile(new CreateFileIn() { Category = "Preview", ContentType = "image/jpeg", File = cropResult, FileName = $"{Guid.NewGuid().ToString()}.jpg" });
            rawDetection.CropFileId = cropFileeeee.Id;

            //var cropFileContent = await _httpClientService.GetImageBytesAsync(cropUrl);
            //var cropFile = await _fileService.UploadFile(new CreateFileIn() { Category = "Crop", ContentType = "image/jpeg", File = cropFileContent, FileName = $"{Guid.NewGuid().ToString()}.jpg" });
            //rawDetection.CropFileId = cropFile.Id;


            await _rawDetectionRepository.SaveOrUpdateAsync(rawDetection);
            await _rawDetectionRepository.CommitChangesAsync();
        }

        private async Task ProcessRawDataInfoAiAsync(Guid rawDetectionId)
        {
            var rawDetection = await _rawDetectionRepository.QueryAll().Include(x => x.Detections).FirstOrDefaultAsync(x => x.Id == rawDetectionId);
            var file = await GetRawDetectionFileAsync(rawDetection.CropFileId.Value);

            var endpoint = "";
            switch (rawDetection.MainClass)
            {
                case "Trash":
                    endpoint = "http://10.254.42.5:8885/process-trash-image";
                    break;
                case "Pothole":
                    endpoint = "http://10.254.42.5:8885/process-potholes-image";
                    break;
                case "Billboard":

                    endpoint = "http://10.254.42.5:8885/process-billboard-image";
                    break;
                case "TrafficSign":
                    endpoint = "http://10.254.42.5:8885/process-trafficsigns-image";
                    break;

            }


            //
            //aici apelat cu crop "din ImageUrl din detection"
            var result = await _httpClientService.GetDataFromFileAiWithFormData(endpoint, file.Content);
            rawDetection.ProcessedExtensionData = result;

            await _rawDetectionRepository.SaveOrUpdateAsync(rawDetection);
            await _rawDetectionRepository.CommitChangesAsync();

        }



        private async Task ImportDetection(RawDetection detection)
        {
            if (!_functionsGenerateDetections.ContainsKey(detection.MainClass))
                throw new ArgumentOutOfRangeException(nameof(detection.MainClass), detection.MainClass, $"Import for {detection.MainClass} is not implemented.");

            var result = await _functionsGenerateDetections[detection.MainClass](detection);
        }

        private async Task<int> ImportRoadInfrastructure(RawDetection detection)
        {
            var dictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(detection.ProcessedExtensionData);
            var roadInfrastructureRequest = JsonConvert.DeserializeObject<RoadInfrastructureDetectionRequest>(dictionaryResult.FirstOrDefault().Value);
            Mapper.Map(detection, roadInfrastructureRequest);

            var files = await RawDataFilesToCreateFile(detection);
            roadInfrastructureRequest.Files = files;

            await _roadInfrastructureService.CreateRoadInfrastructureDetectionAsync(roadInfrastructureRequest);
            return 1;
        }
        private async Task<int> ImportBillBoardDetection(RawDetection detection)
        {
            var dictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(detection.ProcessedExtensionData);
            var billboardRequest = JsonConvert.DeserializeObject<CreateBillBoardDetectionRequest>(dictionaryResult.FirstOrDefault().Value);
            Mapper.Map(detection, billboardRequest);

            var files = await RawDataFilesToCreateFile(detection);
            billboardRequest.Files = files;

            await _billBoardService.CreateBillBoardDetectionAsync(billboardRequest);
            return 1;
        }
        private async Task<int> ImportTrashDetection(RawDetection detection)
        {
            var dictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(detection.ProcessedExtensionData);
            var trashRequest = JsonConvert.DeserializeObject<CreateTrashDetectionRequest>(dictionaryResult.FirstOrDefault().Value);
            Mapper.Map(detection, trashRequest);

            trashRequest.Description = trashRequest.TrashDescription;
            var files = await RawDataFilesToCreateFile(detection);
            trashRequest.Files = files;

            await _trashService.CreateTrashDetectionAsync(trashRequest);
            return 1;
        }

        private async Task<int> ImportTrafficSignDetection(RawDetection detection)
        {
            var dictionaryResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(detection.ProcessedExtensionData);
            var trafficSignRequest = new CreateTrafficSignDetectionRequest();
            var extensionData = JsonConvert.DeserializeObject<TrafficSignExtensionData>(dictionaryResult.FirstOrDefault().Value);
            Mapper.Map(detection, trafficSignRequest);
            trafficSignRequest.Description = extensionData.Description;
            trafficSignRequest.TraficSignCategory = extensionData.Category;
            trafficSignRequest.TraficSignSubClass = detection.SubClass;

            var files = await RawDataFilesToCreateFile(detection);
            trafficSignRequest.Files = files;

            await _trafficSignService.CreateTrafficSignDetectionAsync(trafficSignRequest);
            return 1;
        }




        private async Task<List<CreateFileIn>> RawDataFilesToCreateFile(RawDetection detection)
        {

            var files = new List<CreateFileIn>();

            if (detection.FileId.HasValue)
            {
                var f = await FileToFileIn(detection.FileId.Value);
                files.Add(f);
            }

            if (detection.ProcessedsFileId.HasValue)
            {
                var f = await FileToFileIn(detection.ProcessedsFileId.Value);
                files.Add(f);
            }

            if (detection.PreviewFileId.HasValue)
            {
                var f = await FileToFileIn(detection.PreviewFileId.Value);
                files.Add(f);
            }


            return files;
        }


        private async Task<CreateFileIn> FileToFileIn(Guid fileId)
        {
            var file = await _fileService.GetFile(new GetFileIn() { FileId = fileId });
            var filein = Mapper.Map<CreateFileIn>(file);
            return filein;
        }

        #endregion

        public byte[] ResizeImageKeepAspectRatio(byte[] imageBytes, int maxWidth, int maxHeight)
        {
            using (var inputStream = new MemoryStream(imageBytes))
            using (var originalImage = Image.FromStream(inputStream))
            {
                int originalWidth = originalImage.Width;
                int originalHeight = originalImage.Height;

                // Compute the new size while preserving aspect ratio
                float ratioX = (float)maxWidth / originalWidth;
                float ratioY = (float)maxHeight / originalHeight;
                float ratio = Math.Min(ratioX, ratioY);

                int newWidth = (int)(originalWidth * ratio);
                int newHeight = (int)(originalHeight * ratio);

                using (var bitmap = new Bitmap(newWidth, newHeight))
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);

                    using (var outputStream = new MemoryStream())
                    {
                        bitmap.Save(outputStream, ImageFormat.Jpeg); // You can change the format if needed
                        return outputStream.ToArray();
                    }
                }
            }
        }
    }
}
