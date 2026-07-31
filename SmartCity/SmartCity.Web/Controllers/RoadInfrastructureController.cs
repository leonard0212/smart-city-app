using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Web.Models.RoadInfrastructure;
using SmartCity.Core.MVC.Extensions;
using SmartCity.Web.Models;
using SmartCity.Core.Services;
using SmartCity.Web.Models.Billboard;
namespace SmartCity.Web.Controllers
{


    public class RoadInfrastructureController : SmartCityBaseController
    {
        private readonly IRoadInfrastructureService _roadInfrastructureService;
        private readonly IDetectionService _detectionService;
        public RoadInfrastructureController(IApplicationContext<User> applicationContext
            , IRoadInfrastructureService roadInfrastructureService
            , IDetectionService detectionService) : base(applicationContext)
        {
            _roadInfrastructureService = roadInfrastructureService;
            _detectionService = detectionService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGraphs()
        {

            var serverityPieChart = await _roadInfrastructureService.GetSeverityPieChart();
            var dimensionPieChart = await _roadInfrastructureService.GetDimensionPieChart();
            var typePieChart = await _roadInfrastructureService.GetTypePieChart();
            var response = new
            {
                serverityPieChart = serverityPieChart,
                dimensionPieChart = dimensionPieChart,
                typePieChart = typePieChart
            };
            return Ok(response);
        }

    



        public async Task<IActionResult> GetGridData()
        {
            var detections = await _roadInfrastructureService.GetRoadInfrastructuresData();
            var model = Mapper.Map<List<RoadInfrastructureDataModel>>(detections);
            return PartialView("_GridDataPartial", model);
        }

        public async Task<IActionResult> GetRoadInfrastructureData(Guid detectionId)
        {
            var details = await _roadInfrastructureService.GetRoadInfrastructureDetails(detectionId);
            var model = Mapper.Map<RoadInfrastructureInfoModel>(details);
            return PartialView("_RoadInfrastructureDetails", model);
        }
        






    }
}
