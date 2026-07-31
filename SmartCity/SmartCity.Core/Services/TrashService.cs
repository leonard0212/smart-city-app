using Microsoft.EntityFrameworkCore;
using SmartCity.Core.Utils;
using SmartCity.Domain.Extensions;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.Charts;
using SmartCity.Domain.ServiceModels.Trash;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Core.Services
{
    public class TrashService : ServiceBase, ITrashService
    {
        private readonly IGenericRepositoryUniqueIdentifier<TrashAssetsDetection> _trashRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<DetectionFile> _detectionFileRepository;
        private readonly IFileService _fileService;
        public TrashService(IApplicationContext<User> applicationContext
            , IGenericRepositoryUniqueIdentifier<TrashAssetsDetection> trashRepository
            , IGenericRepositorySimpleUniqueIdentifier<DetectionFile> detectionFileRepository
            , IFileService fileService
            ) : base(applicationContext)
        {
            _trashRepository = trashRepository;
            _detectionFileRepository = detectionFileRepository;
            _fileService = fileService;
        }

        public static readonly IDictionary<int, string> Colors =
       new Dictionary<int, string> {
                    { 0,"#BF5173"},
                    { 1,"#000000"},
                    { 2,"#f08686"},
                    { 3,"#ff0000"},
                    { 4,"#ffffff"},
       };

        public async Task CreateTrashDetectionAsync(CreateTrashDetectionRequest request)
        {
            var trash = Mapper.Map<TrashAssetsDetection>(request);
            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                await _trashRepository.SaveOrUpdateAsync(trash);
                foreach (var file in request.Files)
                {
                    var fileUploaded = await _fileService.UploadFile(file);
                    var detactionFile = new DetectionFile()
                    {
                        DetectionId = trash.Id,
                        FileId = fileUploaded.Id
                    };
                    await _detectionFileRepository.SaveOrUpdateAsync(detactionFile);
                }
                await _trashRepository.CommitChangesAsync();
            });
        }

        public async Task<TrashInfoServiceModel> GetTrashDetails(Guid detectionId)
        {
            var detection = await _trashRepository.GetAsync(detectionId);
            var result = Mapper.Map<TrashInfoServiceModel>(detection);
            return result;
        }


        public async Task<List<TrashInfoServiceModel>> GetTrashDetailsData()
        {
            var detection = await _trashRepository.QueryAll().ToListAsync();
            var result = Mapper.Map<List<TrashInfoServiceModel>>(detection);
            return result;
        }


        public async Task<PieChartModel> GetCategoryPieChart()
        {


            var roadinfra = await _trashRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.TrashAssetCategory).ToList();

            var data = new Dictionary<string, int>();
            var colors = new List<string>();
            foreach (var item in group)
            {
                data.Add(item.Key.Description(), item.Count());
                string color = "";
                if (Colors.TryGetValue((int)item.Key, out color))
                    colors.Add(color);

            }

            var model = ChartsUtils.CreatePieChartModel(data, colors, "Categorie");
            return model;
        }
        public async Task<PieChartModel> GetVolumePieChart()
        {


            var roadinfra = await _trashRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.TrashVolumType).ToList();

            var data = new Dictionary<string, int>();
            var colors = new List<string>();
            foreach (var item in group)
            {
                data.Add(item.Key.Description(), item.Count());
                string color = "";
                if (Colors.TryGetValue((int)item.Key, out color))
                    colors.Add(color);

            }

            var model = ChartsUtils.CreatePieChartModel(data, colors, "Volum");
            return model;
        }

        public async Task<PieChartModel> GetAssetPieChart()
        {
            var roadinfra = await _trashRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.AssetStatus).ToList();

            var data = new Dictionary<string, int>();
            var colors = new List<string>();
            foreach (var item in group)
            {
                data.Add(item.Key.Description(), item.Count());
                string color = "";
                if (Colors.TryGetValue((int)item.Key, out color))
                    colors.Add(color);

            }

            var model = ChartsUtils.CreatePieChartModel(data, colors, "Stare");
            return model;
        }
    }
}
