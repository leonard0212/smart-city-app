using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;

namespace SmartCity.Web.Controllers
{
    public class LogsController : SmartCityBaseController
    {

        private readonly ILogsService _logsService;
        public LogsController(IApplicationContext<User> applicationContext, ILogsService logsService) : base(applicationContext)
        {
            _logsService = logsService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _logsService.GetMessageBusLogs();
            return View(result);
        }


        public async Task<IActionResult> ViewMessageBusLog(long Id)
        {
            var result = await _logsService.GetMessageBusLogById(Id);
            return View(result);

        }
    }
}
