using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Core.Services;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Web.Models.Billboard;

namespace SmartCity.Web.Controllers
{
    public class BillboardController : SmartCityBaseController
    {
        private readonly IBillBoardService _billBoardService;
        public BillboardController(IApplicationContext<User> applicationContext
            , IBillBoardService billBoardService
            ) : base(applicationContext)
        {
            _billBoardService = billBoardService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetBillboardData(Guid detectionId)
        {
            var details = await _billBoardService.GetBilboardDetails(detectionId);
            var model = Mapper.Map<BilboardInfoModel>(details);
            return PartialView("_BillboardDetails", model);
        }

        public async Task<IActionResult> GetBillboardGrid(Guid detectionId)
        {
            var details = await _billBoardService.GetBilboardsDetailsData();
            var model = Mapper.Map<List<BilboardInfoModel>>(details);
            return PartialView("_BillboardGrid", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGraphs()
        {

            var categoryPieChart = await _billBoardService.GetCategoryPieChart();
            var sizePieChart = await _billBoardService.GetSizePieChart();
            //var statusPieChart = await _trashService.GetAssetPieChart();

            var response = new
            {
                categoryPieChart = categoryPieChart,
                sizePieChart = sizePieChart,
                //statusPieChart = statusPieChart

            };
            return Ok(response);
        }

    }
}
