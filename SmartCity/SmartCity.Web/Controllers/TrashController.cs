using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Core.Services;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Web.Models.Billboard;
using SmartCity.Web.Models.Trash;

namespace SmartCity.Web.Controllers
{
    public class TrashController : SmartCityBaseController
    {
        private readonly ITrashService _trashService;
        public TrashController(IApplicationContext<User> applicationContext
            , ITrashService trashService
            ) : base(applicationContext)
        {
            _trashService = trashService;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> GetTrashData(Guid detectionId)
        {
            var details = await _trashService.GetTrashDetails(detectionId);
            var model = Mapper.Map<TrashInfoModel>(details);
            return PartialView("_TrashDetails", model);
        }
        public async Task<IActionResult> GetTrashdGrid(Guid detectionId)
        {
            var details = await _trashService.GetTrashDetailsData();
            var model = Mapper.Map<List<TrashInfoModel>>(details);
            return PartialView("_TrashGrid", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGraphs()
        {

            var categoryPieChart = await _trashService.GetCategoryPieChart();
            var volumePieChart = await _trashService.GetVolumePieChart();
            var statusPieChart = await _trashService.GetAssetPieChart();

            var response = new
            {
                categoryPieChart = categoryPieChart,
                volumePieChart = volumePieChart,
                statusPieChart = statusPieChart

            };
            return Ok(response);
        }


    }
}
