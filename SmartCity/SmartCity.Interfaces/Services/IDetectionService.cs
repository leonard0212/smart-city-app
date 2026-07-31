using SmartCity.Domain.Models.Entities;
using SmartCity.Domain;
using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.Detection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCity.Domain.Models.Detection;
using SmartCity.Domain.ServiceModels.Departament;

namespace SmartCity.Interfaces.Services
{
    public interface IDetectionService
    {
        Task<List<DetectionDataOut>> GetMapsData(DetectionCategory? detectionCategory);
        Task<DetectionDataOut> GetMapsDataById(Guid id);
        Task<DetectionFileServiceModel> GetDetectionProcessedFileById(Guid detectionId);
        Task<List<DetectionFileServiceModel>> GetDetectionsPreviewsFiles(DetectionCategory detectionCategory);


        Task<IPagedList<Detection>> GetIPagedListDetections(DetectionFilter model);

        Task<DetectionDataModel> GetDetectionData(Guid detectionId, bool includeImage = true);



        Task RejectDetection(Guid detectionId, string text);
        Task MarkAsRezolvedDetection(Guid detectionId, string text);
        Task AssignToDepartamentDetection(Guid detectionId, Guid departamentId, Guid teamId, string text, DateTime startDate, DateTime endDate);
        Task<List<GranttDataModel>> GetGanttData(DateTime startDate, DateTime endDate);

    }
}
