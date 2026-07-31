using SmartCity.Domain;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ServiceModels.RawDetection;
using SmartCity.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Services
{
    public interface IRawDetectionService
    {
        Task CreateRawDetectionAsync(string messageBusMessage, string messageId);
        Task ProcessRawDetection(Guid rawDetectionId);
        Task<List<RawDetection>> GetRawDetectionsListAsync();
        Task<string> GetRawDetectionProcessedFileAsync(Guid rawDetectionId, bool isPreview = false, bool isCrop = false);
        Task<FileServiceModel> GetRawDetectionPreviewFileAsync(Guid rawDetectionId);
        Task<Dictionary<string, string>> GetRawDetectionInfosAsync(Guid RawDetectionId);
        Task DeleteRawDataAsync(Guid rawDetectionId);
        Task<(IPagedList<RawDetection> list, List<RawDetectionInfo> info, List<RawDetectionFile> previewFiles, List<RawDetectionFile> processedFiles)> GetIPagedListRawDetectionsListAsync(RawDetectionFilterServiceModel model);
        Task<FileStreamResultModel> GetRawDetectionProcessedStreamImageAsync(Guid rawDetectionId, int? width = null);
        Task DeleteRawDataBatchAsync(IEnumerable<Guid> rawDetectionIds);
    }
}
