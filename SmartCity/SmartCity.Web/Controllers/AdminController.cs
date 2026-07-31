using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Core.Services;
using SmartCity.Domain.Extensions;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.Detection;
using SmartCity.Domain.ServiceModels.File;
using SmartCity.Domain.ServiceModels.RawDetection;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces.Services.Common;
using SmartCity.Web.Models;
using SmartCity.Web.Models.Admin;
using SmartCity.Web.Models.Home;

namespace SmartCity.Web.Controllers
{
    public class AdminController : SmartCityBaseController
    {
        private readonly IRawDetectionService _rawDetectionService;
        private readonly INomenclatureService _nomenclatureService;
        public AdminController(IApplicationContext<User> applicationContext, IRawDetectionService rawDetectionService, INomenclatureService nomenclatureService) : base(applicationContext)
        {
            _rawDetectionService = rawDetectionService;
            _nomenclatureService = nomenclatureService;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> RawDetectionsToProcess()
        {

            var model = new RawDetectionsFilterModel();
            model.EdgeIds = await _nomenclatureService.GetEdgeIds(true);
            model.Category = await _nomenclatureService.GetRawDetectionsCategory(true);

            return View("RawDetectionsToProcessFilter", model);

            //var detections = await _rawDetectionService.GetRawDetectionsListAsync();
            //var viewModel = Mapper.Map<List<RawDetectionViewModel>>(detections);
            //return View(viewModel);
        }

        public async Task<IActionResult> GetRawDetectionsToProcess(RawDetectionsFilterModel model)
        {
            var last = GetLastPageIndex();
            var effectivePage = last ?? model.PageIndex ?? 1;
            var effectivePageSize = model.PageSize ?? 10;

            var reqModel = new RawDetectionFilterServiceModel
            {
                PageIndex = effectivePage,
                PageSize = effectivePageSize,
                EdgeId = model.EdgeId,
                Category = model.CategoryId,
                SubCategory = model.SubCategoryId,
                Status = model.Status
            };

            var resp = await _rawDetectionService.GetIPagedListRawDetectionsListAsync(reqModel);
            var pagedListMapped = resp.list.ToMappedPagedList<RawDetection, RawDetectionViewModel>(Mapper);

            // 2) clamp dacă am rămas peste ultima pagină (ex: ai șters ultimul item de pe pagină)
            var lastPage = Math.Max(1, (int)Math.Ceiling(pagedListMapped.TotalCount / (double)model.PageSize));
            if (effectivePage > lastPage)
            {
                effectivePage = lastPage;
            }

            if (last.HasValue) ClearLastPageIndex();

            var retmodel = new FilteredRawDetectionModel
            {
                list = pagedListMapped,
                info = resp.info,
                processedFiles = resp.processedFiles
            };

            return PartialView("RawDetectionsToProcess", retmodel);
        }




        public async Task<IActionResult> GetRawDetectionSubclass(string mainclass)
        {
            var values = await _nomenclatureService.GetRawDetectionsSubCategory(mainclass, true);
            return Ok(values);
        }


        public async Task<IActionResult> GetRawDetectionPreviewImg(Guid rawDetectionId)
        {
            var file = await _rawDetectionService.GetRawDetectionPreviewFileAsync(rawDetectionId);
            return Ok(file);
        }

        public async Task<IActionResult> GetRawDetectionProcessedImg(Guid rawDetectionId, bool isPreview, bool isCrop = false)
        {
            var fileUrl = await _rawDetectionService.GetRawDetectionProcessedFileAsync(rawDetectionId, isPreview, isCrop);

            return Ok(new { src = fileUrl });
            //return new FileStreamResult(new MemoryStream(file.Content), "image/jpeg");

            //  return Ok(file);
        }
        public async Task<IActionResult> GetRawDetectionPreviewImg(Guid rawDetectionId, bool isPreview)
        {
            var fileUrl = await _rawDetectionService.GetRawDetectionProcessedFileAsync(rawDetectionId, isPreview);

            return Ok(new { src = fileUrl });
            //return new FileStreamResult(new MemoryStream(file.Content), "image/jpeg");

            //  return Ok(file);
        }

        [HttpGet]
        [Route("Admin/GetRawDetectionStreamImg")]
        public async Task<IActionResult> GetRawDetectionStreamImg(Guid rawDetectionId, int? width = null)
        {

            var result = await _rawDetectionService.GetRawDetectionProcessedStreamImageAsync(rawDetectionId, width);
            return File(result.Stream, result.ContentType, result.FileName, enableRangeProcessing: true);
        }

        public async Task<IActionResult> GetRawDetectionInfo(Guid rawDetectionId)
        {
            var info = await _rawDetectionService.GetRawDetectionInfosAsync(rawDetectionId);
            return Ok(info);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRawDetection(Guid rawDetectionId, [FromQuery] int pageIndex = 1)
        {
            try
            {
                await _rawDetectionService.DeleteRawDataAsync(rawDetectionId);

                SetLastPageIndex(pageIndex);
                var resp = new { message = "RawDetection has been deleted", succes = true };
                return Ok(resp);
            }
            catch (Exception ex)
            {
                return Ok(new { message = ex.Message, succes = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRawDetections([FromBody] DeleteDetectionsModel model, [FromQuery] int pageIndex = 1)
        {
            if (model?.RawDetectionIds == null || model.RawDetectionIds.Count == 0)
                return Ok(new { message = "No items selected.", succes = false });

            try
            {
                await _rawDetectionService.DeleteRawDataBatchAsync(model.RawDetectionIds);
                SetLastPageIndex(pageIndex);
                return Ok(new { message = $"{model.RawDetectionIds.Count} RawDetection(s) have been deleted", succes = true });
            }
            catch (Exception ex)
            {
                return Ok(new { message = ex.Message, succes = false });
            }
        }


        public async Task<IActionResult> ImportRawDetection(Guid rawDetectionId, [FromQuery] int pageIndex = 1)
        {
            try
            {
                await _rawDetectionService.ProcessRawDetection(rawDetectionId);
                SetLastPageIndex(pageIndex);
                var resp = new { message = "RawDetection has been procesed", succes = true };
                return Ok(resp);
            }
            catch (Exception ex)
            {
                var resp = new { message = ex.Message, succes = false };
                return Ok(resp);
            }
        }




    }
}
