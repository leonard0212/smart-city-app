using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Controllers;
using SmartCity.Domain;
using SmartCity.Domain.Models.Entities;
using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.Departament;
using SmartCity.Domain.ServiceModels.User;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Web.Models.Departament;
using SmartCity.Web.Models.Home;
using SmartCity.Web.Models.UserManagement;
using SmartCity.Domain.Extensions;
using NLog.Filters;
using SmartCity.Core.Services;
using SmartCity.Interfaces.Repository;
using Microsoft.AspNetCore.Identity;
using System.Xml.Linq;

namespace SmartCity.Web.Controllers
{
    public class DepartmentController : SmartCityBaseController
    {
        private readonly IDepartamentService _departamentService;
        private readonly UserManager<User> _userManager;
        private readonly IDetectionService _detectionService;
        public DepartmentController(IApplicationContext<User> applicationContext, IDepartamentService departamentService
            , UserManager<User> userManager
            , IDetectionService detectionService
            ) : base(applicationContext)
        {
            _departamentService = departamentService;
            _userManager = userManager;
            _detectionService = detectionService;
        }

        public async Task<IActionResult> Index()
        {

            var model = new DepartamentFilterViewModel();
            return View(model);

        }
        public async Task<IActionResult> GetDepartaments(DepartamentFilterViewModel model)
        {


            var filter = Mapper.Map<DepartamentFilterModel>(model);
            var results = await _departamentService.GetIPagedListDepartaments(filter);

            var pagedListMapped = results.ToMappedPagedList<Departament, DepartamentFilteredViewModel>(Mapper);
            return PartialView("_DepartamentFilteredPartial", pagedListMapped);

        }


        public async Task<IActionResult> AddDepartment(string Name)
        {
            await _departamentService.CreateDepartament(Name);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddTeam(Guid Id, string Name)
        {
            await _departamentService.CreateTeam(Id, Name);
            return RedirectToAction(nameof(DepartamentInfo), new { Id = Id });
        }



        public async Task<IActionResult> DepartamentInfo(Guid Id)
        {
            var departamentInfo = await _departamentService.GetDepartamentById(Id);
            var model = Mapper.Map<DepartamentInfoViewModel>(departamentInfo);
            return View(model);
        }

        public async Task<IActionResult> TeamInfo(Guid Id)
        {
            var teamInfo = await _departamentService.GetTeamById(Id);
            var model = Mapper.Map<TeamInfoViewModel>(teamInfo.Item1);
            var existingUsers = model.UserMembership.Select(x => x.Key);
            var usersNotExists = _userManager.Users.Where(x => !existingUsers.Contains(x.Id));
            model.NotUserMembership = usersNotExists?.ToDictionary(x => x.Id, x => x.UserName);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddUserToTeam(Guid teamId, Guid userId)
        {
            await _departamentService.AddUserToTeam(teamId, userId);
            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> RemoveUserFromTeam(Guid teamId, Guid userId)
        {
            await _departamentService.RemoveUserFromTeam(teamId, userId);
            return Ok();
        }






        public async Task<IActionResult> Gantt(GanttViewModel model)
        {

            model = model ?? new GanttViewModel();


            var gantdata = await _detectionService.GetGanttData(model.BeginDate, model.EndDate);
            model.GranttData = gantdata;

        

            return View("DepartamentDispatcherView", model);
        }

    }
}

