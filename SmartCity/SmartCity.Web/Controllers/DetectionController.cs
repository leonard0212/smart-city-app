using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartCity.Core.Controllers;
using SmartCity.Core.Services;
using SmartCity.Database;
using SmartCity.Domain.Models.Detection;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Common;
using SmartCity.Web.Models.Detection;

namespace SmartCity.Web.Controllers
{
    public class DetectionController : SmartCityBaseController
    {

        private readonly IDetectionService _detectionService;
        private readonly INomenclatureService _nomenclatureService;
        private readonly IDetectionFlowLogService _detectionFlowLogService;
        public DetectionController(IApplicationContext<User> applicationContext
            , IDetectionService detectionService
            , INomenclatureService nomenclatureService
            , IDetectionFlowLogService detectionFlowLogService) : base(applicationContext)
        {
            _detectionService = detectionService;
            _nomenclatureService = nomenclatureService;
            _detectionFlowLogService = detectionFlowLogService;
        }



        public async Task<IActionResult> Index(Guid Id)
        {
            var detectionData = await _detectionService.GetDetectionData(Id);
            var model = Mapper.Map<DetectionViewModel>(detectionData);
            model.FlowData = await CreateFlowModel(Id);
            return View(model);
        }


        public async Task<IActionResult> GetDetectionFlowDataHome(Guid Id)
        {
            var model = await CreateFlowModel(Id);
            return PartialView("_Flow2Partial", model);
        }

        public async Task<IActionResult> GetDetectionFlowData(Guid Id)
        {
            var model = await CreateFlowModel(Id);
            return PartialView("_FlowPartial", model);
        }


        private async Task<DetectionFlowViewModel> CreateFlowModel(Guid Id)
        {
            var model = new DetectionFlowViewModel();
            model.DetectionId = Id;
            var logs = await _detectionFlowLogService.GetDetectionFlowLog(Id);
            foreach (var log in logs)
            {
                model.Logs.Add(new DetectionLogViewModel
                {
                    LogText = log.LogText,
                    CreatedAt = log.CreatedAt,
                    User = log.User?.UserName
                });
            }

            if (!model.Logs.Any())
            {
                model.Logs.Add(new DetectionLogViewModel
                {
                    LogText = " Detectia nu a fost atribuita catre departament.",
                    CreatedAt = DateTime.UtcNow.AddHours(3),
                    User = "System"
                });
                model.Logs.Add(new DetectionLogViewModel
                {
                    LogText = " Pentru a remedia problema, atribuiti detectia catre departamentul corespunzator.",
                    CreatedAt = DateTime.UtcNow.AddHours(3),
                    User = "System"
                });
            }



            model.AssignToDepartment.Departaments = await _nomenclatureService.GetDepartamentsList(true);
            return model;
        }


        public async Task<IActionResult> AddDetectionChat(Guid detectionId, string detectionText)
        {
            await _detectionFlowLogService.CreateDetectionFlowLog(detectionId, SecurityContext.User.Id, detectionText);
            var model = await CreateFlowModel(detectionId);
            return PartialView("_Flow2Partial", model);
        }




        public async Task<IActionResult> RejectDetection(Guid detectionId, string text)
        {
            await _detectionService.RejectDetection(detectionId, text);
            var model = await CreateFlowModel(detectionId);
            return PartialView("_Flow2Partial", model);
        }

        public async Task<IActionResult> MarkAsRezolvedDetection(Guid detectionId, string text)
        {
            await _detectionService.MarkAsRezolvedDetection(detectionId, text);
            var model = await CreateFlowModel(detectionId);
            return PartialView("_Flow2Partial", model);
        }



        public async Task<IActionResult> AssignToDepartamentDetection(Guid detectionId, Guid departamentId, Guid teamId, string text, DateTime startDate, DateTime endDate)
        {
            await _detectionService.AssignToDepartamentDetection(detectionId, departamentId, teamId, text, startDate, endDate);
            var model = await CreateFlowModel(detectionId);
            return PartialView("_Flow2Partial", model);
        }


        public async Task<IActionResult> GetDetectionInfoData(Guid Id)
        {
            var detectionData = await _detectionService.GetDetectionData(Id, false);
            var model = Mapper.Map<DetectionViewModel>(detectionData);
            return PartialView("_GeneralInformationForGanttPartial", model);
        }
    }
}
