using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartCity.Core.Services.Common;
using SmartCity.Core.Utils;
using SmartCity.Domain.Extensions;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.BillBoard;
using SmartCity.Domain.ServiceModels.Charts;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Intaro.Contracts.Services;
namespace SmartCity.Core.Services
{
    public class BillBoardService : ServiceBase, IBillBoardService
    {
        private readonly IGenericRepositoryUniqueIdentifier<BillboardDetection> _billBoardDetectionRepository;
        private readonly IGenericRepositorySimpleUniqueIdentifier<DetectionFile> _detectionFileRepository;
        private readonly IFileService _fileService;
        public static readonly IDictionary<int, string> Colors =
         new Dictionary<int, string> {
                    { 0,"#BF5173"},
                    { 1,"#000000"},
                    { 2,"#f08686"},
                    { 3,"#ff0000"},
                    { 4,"#ffffff"},
         };

        public BillBoardService(IApplicationContext<User> applicationContext
            , IGenericRepositoryUniqueIdentifier<BillboardDetection> billBoardDetectionRepository
            , IGenericRepositorySimpleUniqueIdentifier<DetectionFile> detectionFileRepository
            , IFileService fileService
            ) : base(applicationContext)
        {
            _billBoardDetectionRepository = billBoardDetectionRepository;
            _detectionFileRepository = detectionFileRepository;
            _fileService = fileService;
        }


        public async Task CreateBillBoardDetectionAsync(CreateBillBoardDetectionRequest request)
        {
            //TODO: validate input
            var billboard = Mapper.Map<BillboardDetection>(request);
            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                await _billBoardDetectionRepository.SaveOrUpdateAsync(billboard);
                foreach (var file in request.Files)
                {
                    var fileUploaded = await _fileService.UploadFile(file);
                    var detactionFile = new DetectionFile()
                    {
                        DetectionId = billboard.Id,
                        FileId = fileUploaded.Id
                    };
                    await _detectionFileRepository.SaveOrUpdateAsync(detactionFile);
                }
                await _billBoardDetectionRepository.CommitChangesAsync();
            });
        }

        public async Task<BilboardInfoServiceModel> GetBilboardDetails(Guid detectionId)
        {
            var detection = await _billBoardDetectionRepository.GetAsync(detectionId);
            var result = Mapper.Map<BilboardInfoServiceModel>(detection);
            return result;
        }

        public async Task<List<BilboardInfoServiceModel>> GetBilboardsDetailsData()
        {
            var detection = await _billBoardDetectionRepository.QueryAll().ToListAsync();
            var result = Mapper.Map<List<BilboardInfoServiceModel>>(detection);
            return result;
        }



        public async Task<PieChartModel> GetCategoryPieChart()
        {


            var roadinfra = await _billBoardDetectionRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.BillboardCategory).ToList();

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



        public async Task<PieChartModel> GetSizePieChart()
        {


            var roadinfra = await _billBoardDetectionRepository.QueryAll().ToListAsync();
            var group = roadinfra.GroupBy(x => x.BillboardSize).ToList();

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


    }

}
