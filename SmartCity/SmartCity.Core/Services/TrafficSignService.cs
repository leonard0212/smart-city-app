using AutoMapper;
using SmartCity.Database;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.ServiceModels.TraficSign;
using SmartCity.Domain.ServiceModels.Trash;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;
using SmartCity.Domain.Models.Users;

namespace SmartCity.Core.Services
{
    public class TrafficSignService : ServiceBase, ITrafficSignService
    {
        private readonly IGenericRepositoryUniqueIdentifier<TrafficSignDetection> _trafficSignRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<DetectionFile> _detectionFileRepository;
        private readonly IFileService _fileService;
        public TrafficSignService(IApplicationContext<User> applicationContext
            , IGenericRepositoryUniqueIdentifier<TrafficSignDetection> trafficSignRepository
            , IGenericRepositorySimpleUniqueIdentifier<DetectionFile> detectionFileRepository
            , IFileService fileService
            ) : base(applicationContext)
        {
            _trafficSignRepository = trafficSignRepository;
            _detectionFileRepository = detectionFileRepository;
            _fileService = fileService;
        }

        public async Task CreateTrafficSignDetectionAsync(CreateTrafficSignDetectionRequest request)
        {
            var trafficSign = Mapper.Map<TrafficSignDetection>(request);
            trafficSign.Category = request.TraficSignCategory;
            trafficSign.Subclass = request.TraficSignSubClass;

            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                await _trafficSignRepository.SaveOrUpdateAsync(trafficSign);
                foreach (var file in request.Files)
                {
                    var fileUploaded = await _fileService.UploadFile(file);
                    var detactionFile = new DetectionFile()
                    {
                        DetectionId = trafficSign.Id,
                        FileId = fileUploaded.Id
                    };
                    await _detectionFileRepository.SaveOrUpdateAsync(detactionFile);
                }
                await _trafficSignRepository.CommitChangesAsync();
            });
        }
    }
}
