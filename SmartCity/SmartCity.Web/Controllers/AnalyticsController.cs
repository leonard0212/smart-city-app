using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Core.Services;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;

namespace SmartCity.Web.Controllers
{
    public class AnalyticsController : SmartCityBaseController
    {
        private readonly PowerBIService _powerBIService = new PowerBIService();
        public AnalyticsController(IApplicationContext<User> applicationContext) : base(applicationContext)
        {
        }


        public async Task<IActionResult> Index()
        {
            //var embedToken = await _powerBIService.GetPowerBIEmbedToken();
            //ViewBag.EmbedToken = embedToken.Token;
            //ViewBag.ReportId = "0dd0b44c-db02-4e6d-82b2-e51930bac484";
            //ViewBag.EmbedUrl = "YOUR_EMBED_URL";


            return View();
        }
    }
}
