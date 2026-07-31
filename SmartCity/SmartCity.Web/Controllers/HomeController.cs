using SmartCity.Core.Controllers;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.Extensions;
using SmartCity.Interfaces;
using SmartCity.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using SmartCity.Web.Models.Home;
using Microsoft.Data.SqlClient;
using Dapper;
using SmartCity.Core.Extensions;
using SmartCity.Interfaces.Services;
using SmartCity.Core.Services;
using SmartCity.Domain.ServiceModels.Detection;
using SmartCity.Domain.Models.Entities;
using SmartCity.Interfaces.Services.Common;

namespace SmartCity.Web.Controllers
{
    public class HomeController : SmartCityBaseController
    {

        private readonly IConfiguration _configuration;
        private readonly IDetectionService _detectionService;
        private readonly INomenclatureService _nomenclatureService;
        public HomeController(IApplicationContext<User> applicationContext, ILogger<HomeController> logger, IConfiguration configuration, IDetectionService detectionService, INomenclatureService nomenclatureService) : base(applicationContext)

        {
            _detectionService = detectionService;
            _configuration = configuration;
            _nomenclatureService = nomenclatureService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await createModel();

            return View(model);
        }

        private async Task<HomeModel> createModel()
        {
            var model = new HomeModel();
            var _connectionString = _configuration["database:connection"];


            using (var connection = new SqlConnection(_connectionString))
            {
                model.DetectionCount = await connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM Detection");
                model.DetectionUnalocatedCount = model.DetectionCount;

                model.TrashDetectionCount = await connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM Detection WHERE Category = 'Garbage'");
                model.TrashDetectionUnalocatedCount = model.TrashDetectionCount;

                model.BillboardDetectionCount = await connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM Detection WHERE Category = 'Billboards'");
                model.BillboardDetectionUnalocatedCount = model.BillboardDetectionCount;

                model.RoadInfrastructureDetectionCount = await connection.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM Detection WHERE Category = 'RoadInfrastructure'");
                model.RoadInfrastructureDetectionUnalocatedCount = model.RoadInfrastructureDetectionCount;
            }


            model.DetectionFilterModel.Departaments = await _nomenclatureService.GetDepartamentsList(true);




            return model;


        }



        public async Task<IActionResult> FilterDetections(DetectionFilterModel model)
        {
            var reqModel = Mapper.Map<DetectionFilter>(model);
            var resp = await _detectionService.GetIPagedListDetections(reqModel);
            var pagedListMapped = resp.ToMappedPagedList<Detection, DetectionFilteredModel>(Mapper);

            return PartialView("_DetectionFilteredPartial", pagedListMapped);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
