using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using SmartCity.Core.Extensions;
using SmartCity.Core.Utils;
using SmartCity.Domain;
using SmartCity.Domain.Models.Common;
using SmartCity.Domain.Models.Detection;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.Charts;
using SmartCity.Domain.ServiceModels.Departament;
using SmartCity.Domain.ServiceModels.Detection;
using SmartCity.Domain.ServiceModels.RoadInfrastructure;
using SmartCity.Domain.Utils;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices.Marshalling;

namespace SmartCity.Core.Services
{
    public class DetectionService : ServiceBase, IDetectionService
    {
        private readonly IGenericRepositorySimpleUniqueIdentifier<Detection> _detectionRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<Team> _teamRepository;
        private readonly IDetectionRepository _detectionRepository2;
        private readonly IGenericRepositorySimpleUniqueIdentifier<DetectionFile> _detectionFileRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<AppFile> _appFileRepository;
        private readonly IFileService _fileService;
        private readonly ITrashService _trashService;
        private readonly IBillBoardService _billBoardService;
        private readonly IRoadInfrastructureService _roadInfrastructureService;
        private readonly IDetectionFlowLogService _detectionFlowLogService;
        private readonly IGenericRepositorySimpleUniqueIdentifier<Departament> _departmentRepository;

        public DetectionService(IApplicationContext<User> applicationContext
            , IGenericRepositorySimpleUniqueIdentifier<Detection> detectionRepository
            , IGenericRepositorySimpleUniqueIdentifier<Team> teamRepository
            , IDetectionRepository detectionRepository2
            , IGenericRepositorySimpleUniqueIdentifier<DetectionFile> detectionFileRepository
            , IGenericRepositorySimpleUniqueIdentifier<AppFile> appFileRepository
            , IFileService fileService
            , ITrashService trashService
            , IBillBoardService billBoardService
            , IRoadInfrastructureService roadInfrastructureService
            ,
            IDetectionFlowLogService detectionFlowLogService
            ,
            IGenericRepositorySimpleUniqueIdentifier<Departament> departmentRepository

            ) : base(applicationContext)
        {
            _detectionRepository = detectionRepository;
            _teamRepository = teamRepository;
            _detectionRepository2 = detectionRepository2;
            _detectionFileRepository = detectionFileRepository;
            _appFileRepository = appFileRepository;
            _fileService = fileService;
            _trashService = trashService;
            _billBoardService = billBoardService;
            _roadInfrastructureService = roadInfrastructureService;
            _detectionFlowLogService = detectionFlowLogService;
            _departmentRepository = departmentRepository;
        }


        public async Task<List<DetectionDataOut>> GetMapsData(DetectionCategory? detectionCategory)
        {
            var where = QueryPredicateBuilder.True<Detection>();
            if (detectionCategory.HasValue)
                where = where.And(x => x.Category == detectionCategory);

            var data = await _detectionRepository.QueryAll().Where(where).ToListAsync();
            var response = Mapper.Map<List<DetectionDataOut>>(data);
            return response;
        }

        public async Task<DetectionDataOut> GetMapsDataById(Guid id)
        {
            var data = await _detectionRepository.QueryAll().FirstOrDefaultAsync(x => x.Id == id);
            var response = Mapper.Map<DetectionDataOut>(data);
            return response;
        }

        public async Task<DetectionFileServiceModel> GetDetectionProcessedFileById(Guid detectionId)
        {

            var detectionFiles = await _detectionFileRepository.QueryAll().Where(x => x.DetectionId == detectionId).ToListAsync();
            var fileIds = detectionFiles.Select(x => x.FileId);
            var previewFile = await _appFileRepository.QueryAll().Where(x => x.Category == "ProcessedDetection" && fileIds.Contains(x.Id)).FirstOrDefaultAsync();
            if (previewFile == null)
                previewFile = await _appFileRepository.QueryAll().Where(x => fileIds.Contains(x.Id)).FirstOrDefaultAsync();

            var rawDataId = await _detectionRepository.QueryAll().Where(d => d.Id == detectionId).Select(d => d.RawDataId).FirstOrDefaultAsync();

            var file = await _fileService.GetFile(new Domain.ServiceModels.File.GetFileIn() { FileId = previewFile.Id });
            var result = new DetectionFileServiceModel()
            {
                File = file,
                DetectionId = detectionId,
                RawDataId = (Guid)rawDataId
            };
            return result;
        }
        public async Task<List<DetectionFileServiceModel>> GetDetectionsPreviewsFiles(DetectionCategory detectionCategory)
        {


            var files = new List<DetectionFileServiceModel>();
            var detections = await _detectionRepository.QueryAll().Where(x => x.Category == detectionCategory).ToListAsync();
            foreach (var item in detections)
            {
                var detectionFiles = await _detectionFileRepository.QueryAll().Where(x => x.DetectionId == item.Id).ToListAsync();
                var fileIds = detectionFiles.Select(x => x.FileId);
                var previewFile = await _appFileRepository.QueryAll().Where(x => x.Category == "Preview" && fileIds.Contains(x.Id)).FirstOrDefaultAsync();
                if (previewFile == null)
                    previewFile = await _appFileRepository.QueryAll().Where(x => fileIds.Contains(x.Id)).FirstOrDefaultAsync();


                if (previewFile == null)
                    continue;

                var file = await _fileService.GetFile(new Domain.ServiceModels.File.GetFileIn() { FileId = previewFile.Id });
                files.Add(new DetectionFileServiceModel()
                {
                    File = file,
                    DetectionId = item.Id
                });
            }
            return files;
        }

        public async Task<IPagedList<Detection>> GetIPagedListDetections(DetectionFilter model)
        {
            var filterResult = FilterDetections(model);
            var totalCount = await _detectionRepository.CountAsync(filterResult.where);

            var pagedList = await _detectionRepository
              .GetPagedAsync(model.PageIndex.Value, model.PageSize.Value, filterResult.where, filterResult.order, x => x.Departament, x => x.Team)
              .ToPagedListAsync(model.PageIndex.Value, model.PageSize.Value, totalCount, null);

            return pagedList;
        }
        private (Expression<Func<Detection, bool>> where, Func<IQueryable<Detection>, IOrderedQueryable<Detection>> order) FilterDetections(DetectionFilter model)
        {
            model.PageIndex = model.PageIndex > 0 ? model.PageIndex : 1;
            model.PageSize = model.PageSize > 0 ? model.PageSize : PagedList.DefaultPageSize;
            var where = QueryPredicateBuilder.True<Detection>();


            if (model.Category != null)
                where = where.And(d => d.Category == model.Category);
            if (model.Severity != null)
                where = where.And(d => d.Severity == model.Severity);
            if (model.NeedIntervention != null)
                where = where.And(d => d.NeedIntervention == model.NeedIntervention);
            if (model.ResolutionStatus != null)
                where = where.And(d => d.ResolutionStatus == model.ResolutionStatus);
            if (model.ReportingDateStart != null)
                where = where.And(d => d.ReportingDate >= model.ReportingDateStart);
            if (model.ReportingDateEnd != null)
                where = where.And(d => d.ReportingDate <= model.ReportingDateEnd);

            if (model.DepartamentId != null)
                where = where.And(d => d.DepartamentId == model.DepartamentId);


            var order = QueryOrderBuilder.Create<Detection>(query => query.OrderByDescending(x => x.CreatedAt));
            return (where, order);
        }




        public async Task<DetectionDataModel> GetDetectionData(Guid detectionId, bool includeImage = true)
        {
            var data = await _detectionRepository.QueryAll().Where(x=>x.Id == detectionId).Include(x=>x.Departament).Include(x=>x.Team).FirstOrDefaultAsync();
            var response = Mapper.Map<DetectionDataModel>(data);

            if (includeImage)
            {
                var file = await GetDetectionProcessedFileById(detectionId);
                response.FileDatabase64 = Convert.ToBase64String(file.File.Content);
            }
          


            response.SpecificInfo = await GetDetectionSpecificData(detectionId, data.Category);



            return response;
        }


        public async Task<Dictionary<string, object>> GetDetectionSpecificData(Guid detectionId, DetectionCategory detectionCategory)
        {
            dynamic result = null;

            switch (detectionCategory)
            {
                case DetectionCategory.Garbage:
                    result = await _trashService.GetTrashDetails(detectionId);
                    break;
                case DetectionCategory.Billboards:
                    result = await _billBoardService.GetBilboardDetails(detectionId);
                    break;
                case DetectionCategory.RoadInfrastructure:
                    result = await _roadInfrastructureService.GetRoadInfrastructureDetails(detectionId);
                    break;

            }

            var aaaa = DynamicUtils.Dyn2Dict(result);
            return aaaa;
        }

        public async Task RejectDetection(Guid detectionId, string text)
        {
            var detection = await _detectionRepository.GetAsync(detectionId);
            detection.ResolutionStatus = DetectionResolutionStatus.Rejected;
            var logtext = $"Detectare respinsă: \n{text}";
            await _detectionFlowLogService.CreateDetectionFlowLog(detectionId, SecurityContext.User.Id, logtext);
            await _detectionRepository.UpdateAsync(detection);
            await _detectionRepository.CommitChangesAsync();
        }
        public async Task MarkAsRezolvedDetection(Guid detectionId, string text)
        {
            var detection = await _detectionRepository.GetAsync(detectionId);
            detection.ResolutionStatus = DetectionResolutionStatus.Solved;
            var logtext = $"Detectare rezolvată:{text}";
            await _detectionFlowLogService.CreateDetectionFlowLog(detectionId, SecurityContext.User.Id, logtext);
            await _detectionRepository.UpdateAsync(detection);
            await _detectionRepository.CommitChangesAsync();
        }

        public async Task AssignToDepartamentDetection(Guid detectionId, Guid departamentId, Guid teamId, string text, DateTime startDate, DateTime endDate)
        {
            var detection = await _detectionRepository.GetAsync(detectionId);
            var department = await _departmentRepository.GetAsync(departamentId);
            var team = await _teamRepository.GetAsync(teamId);
            detection.ResolutionStatus = DetectionResolutionStatus.InProgress;
            detection.DepartamentId = departamentId;
            detection.TeamId = teamId;

            detection.StartDateResolution = startDate;
            detection.EndDateResolution =endDate;

            var logtext = $"Detectare atribuită departamentului {department.Name}, echipa {team.Name}:{text}";
            await _detectionFlowLogService.CreateDetectionFlowLog(detectionId, SecurityContext.User.Id, logtext);
            await _detectionRepository.UpdateAsync(detection);
            await _detectionRepository.CommitChangesAsync();

        }



        public async Task<List<GranttDataModel>> GetGanttData(DateTime startDate, DateTime endDate)
        {
            var result = new List<GranttDataModel>();
            var queryResult = await _detectionRepository.QueryAll().Where(x => x.StartDateResolution >= startDate && x.StartDateResolution < endDate).Include(x => x.Departament).Include(x => x.Team).ToListAsync();

            if (!queryResult.Any())
            {
                var earliestStart = await _detectionRepository.QueryAll()
                    .Where(x => x.StartDateResolution != null)
                    .MinAsync(x => x.StartDateResolution.Value);

                queryResult = await _detectionRepository.QueryAll()
                    .Where(x => x.StartDateResolution >= earliestStart)
                    .Include(x => x.Departament)
                    .Include(x => x.Team)
                    .ToListAsync();
            }

            foreach (var item in queryResult)
            {
                var d = new GranttDataModel
                {
                    TaskId = item.Id.ToString(),
                    TaskName = $"{item.Team.Name}/{item.Category.ToString()}",
                    Resource = item.Team.Name,
                    Start = item.StartDateResolution.Value,
                    End = item.EndDateResolution.Value,


                };
                result.Add(d);
            }

            return result;
        }
    }
}
