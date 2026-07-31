using Microsoft.Azure.Amqp.Framing;
using Microsoft.EntityFrameworkCore;
using SmartCity.Core.Utils;
using SmartCity.Domain.Extensions;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.Charts;
using SmartCity.Domain.ServiceModels.PotholeDetection;
using SmartCity.Domain.ServiceModels.RoadInfrastructure;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;


namespace SmartCity.Core.Services
{
    public class RoadInfrastructureService : ServiceBase, IRoadInfrastructureService
    {

        public static readonly IDictionary<SeverityType, string> SeverityTypeColors =
           new Dictionary<SeverityType, string> {
                    { SeverityType.Medium,"#BF5173"},
                    { SeverityType.High,"#000000"},
                    { SeverityType.Low,"#f08686"},
                    { SeverityType.Critical,"#ff0000"},
                    { SeverityType.None,"#ffffff"},
           };


        public static readonly IDictionary<int, string> Colors =
          new Dictionary<int, string> {
                    { 0,"#BF5173"},
                    { 1,"#000000"},
                    { 2,"#f08686"},
                    { 3,"#ff0000"},
                    { 4,"#ffffff"},
          };




        private readonly IFileService _fileService;
        private readonly IGenericRepositoryUniqueIdentifier<RoadInfrastructureDetection> _roadInfrastructureDetectionRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<DetectionFile> _detectionFileRepository;
        public RoadInfrastructureService(IApplicationContext<User> applicationContext
            , IGenericRepositoryUniqueIdentifier<RoadInfrastructureDetection> roadInfrastructureRepository
            , IGenericRepositorySimpleUniqueIdentifier<DetectionFile> detectionFileRepository
            , IFileService fileService
            ) : base(applicationContext)
        {
            _roadInfrastructureDetectionRepository = roadInfrastructureRepository;
            _detectionFileRepository = detectionFileRepository;
            _fileService = fileService;
        }


        public async Task CreateRoadInfrastructureDetectionAsync(RoadInfrastructureDetectionRequest request)
        {
            //todo: validate request

            var potholeDetection = Mapper.Map<RoadInfrastructureDetection>(request);
            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                await _roadInfrastructureDetectionRepository.SaveOrUpdateAsync(potholeDetection);
                foreach (var file in request.Files)
                {
                    var fileUploaded = await _fileService.UploadFile(file);
                    var detactionFile = new DetectionFile()
                    {
                        DetectionId = potholeDetection.Id,
                        FileId = fileUploaded.Id
                    };
                    await _detectionFileRepository.SaveOrUpdateAsync(detactionFile);
                }
                await _roadInfrastructureDetectionRepository.CommitChangesAsync();
            });
        }


        public async Task<List<RoadInfrastructureDataOut>> GetRoadInfrastructuresData()
        {
            var roadinfra = await _roadInfrastructureDetectionRepository.QueryAll().ToListAsync();
            var response = Mapper.Map<List<RoadInfrastructureDataOut>>(roadinfra);
            return response;
        }

        public async Task<RoadInfrastructureInfoServiceModel> GetRoadInfrastructureDetails(Guid detectionId)
        {
            var detection = await _roadInfrastructureDetectionRepository.GetAsync(detectionId);
            var result = Mapper.Map<RoadInfrastructureInfoServiceModel>(detection);
            return result;
        }




        #region Charts
        public async Task<PieChartModel> GetSeverityPieChart()
        {


            var roadinfra = await _roadInfrastructureDetectionRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.Severity).ToList();

            var data = new Dictionary<string, int>();
            var colors = new List<string>();
            foreach (var item in group)
            {
                data.Add(item.Key.Description(), item.Count());
                string color = "";
                if (SeverityTypeColors.TryGetValue(item.Key, out color))
                    colors.Add(color);

            }

            var model = ChartsUtils.CreatePieChartModel(data, colors, "Gravitate");
            return model;
        }

        public async Task<PieChartModel> GetDimensionPieChart()
        {


            var roadinfra = await _roadInfrastructureDetectionRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.Dimension).ToList();

            var data = new Dictionary<string, int>();
            var colors = new List<string>();
            foreach (var item in group)
            {
                data.Add(item.Key.Description(), item.Count());
                string color = "";
                if (Colors.TryGetValue((int)item.Key, out color))
                    colors.Add(color);

            }

            var model = ChartsUtils.CreatePieChartModel(data, colors, "Dimensiune");
            return model;
        }

        public async Task<PieChartModel> GetTypePieChart()
        {


            var roadinfra = await _roadInfrastructureDetectionRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.Type).ToList();

            var data = new Dictionary<string, int>();
            var colors = new List<string>();
            foreach (var item in group)
            {
                data.Add(item.Key.Description(), item.Count());
                string color = "";
                if (Colors.TryGetValue((int)item.Key, out color))
                    colors.Add(color);

            }

            var model = ChartsUtils.CreatePieChartModel(data, colors, "Tip");
            return model;
        }



        


        #endregion

    }

}
