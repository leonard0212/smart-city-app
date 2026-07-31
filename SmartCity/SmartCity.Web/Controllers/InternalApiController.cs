using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Core.Services;
using SmartCity.Domain.Models.Enums;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Common;
using SmartCity.Web.Models;

namespace SmartCity.Web.Controllers
{
    public class InternalApiController : SmartCityBaseController
    {
        private readonly IDetectionService _detectionService;
        private readonly IDetectionChatService _detectionChatService;
        private readonly INomenclatureService _nomenclatureService;
        public InternalApiController(
                IApplicationContext<User> applicationContext
              , IDetectionService detectionService
              , IDetectionChatService detectionChatService
              , INomenclatureService nomenclatureService
            ) : base(applicationContext)
        {
            _detectionService = detectionService;
            _detectionChatService = detectionChatService;
            _nomenclatureService = nomenclatureService;
        }
        public async Task<IActionResult> GetMapsData(DetectionCategory? detectionCategory)
        {
            var detections = await _detectionService.GetMapsData(detectionCategory);
            var model = Mapper.Map<List<GmapDataModel>>(detections);
            return Ok(model);
        }
        public async Task<IActionResult> GetMapsDataById(Guid Id)
        {
            var detection = await _detectionService.GetMapsDataById(Id);
            var model = Mapper.Map<GmapDataModel>(detection);
            return Ok(model);
        }

        public async Task<IActionResult> GetDetectionProcessedPicture(Guid Id)
        {
            var file = await _detectionService.GetDetectionProcessedFileById(Id);
            var filedata = Convert.ToBase64String(file.File.Content);
            var ret = new
            {
                filedata = filedata,
                rawDataId = file.RawDataId 
            };
            return Ok(ret);
        }


        public async Task<IActionResult> GetCaruselDetectionData(DetectionCategory detectionCategory)
        {
            var detections = await _detectionService.GetDetectionsPreviewsFiles(detectionCategory);
            return PartialView("_CaruselDetectionPhotosPartial", detections);
        }

        public async Task<IActionResult> GetDetectionDataPartial(Guid Id)
        {
            var detection = await _detectionService.GetMapsDataById(Id);
            var model = Mapper.Map<DetectionDataModel>(detection);
            return PartialView("_CategoryDetailsPartial", model);
        }

        public async Task<IActionResult> GetDetectionChat(Guid detectionId)
        {
            var model = await _detectionChatService.GetChatByDetectionIdAsync(detectionId);
            //   var model = Mapper.Map<RoadInfrastructureInfoModel>(details);
            return PartialView("_DetectionChatPartial", model);
        }

        public async Task<IActionResult> AddDetectionChat(Guid detectionId, string detectionText)
        {
            await _detectionChatService.AddDetectionChatAsync(new Domain.ServiceModels.DetectionChat.DetectionChatIn
            {
                DetectionId = detectionId,
                Text = detectionText
            });
            var model = await _detectionChatService.GetChatByDetectionIdAsync(detectionId);
            //   var model = Mapper.Map<RoadInfrastructureInfoModel>(details);
            return PartialView("_DetectionChatPartial", model);
        }


        public async Task<IActionResult> GetTeamsByDepartmentId(Guid deparmentId)
        {
            var result = await _nomenclatureService.GetTeamsByDepartmentList(deparmentId);
            return Ok(result);
        }



    }
}
