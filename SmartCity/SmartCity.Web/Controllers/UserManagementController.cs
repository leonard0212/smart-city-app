using SmartCity.Core.Controllers;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services.Common;
using SmartCity.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Web.Models.UserManagement;
using SmartCity.Domain.ServiceModels.User;
using Microsoft.Azure.Amqp.Framing;
using SmartCity.Domain;

namespace SmartCity.Web.Controllers
{
    public class UserManagementController : SmartCityBaseController
    {
        private readonly INomenclatureService _nomenclatureService;
        private readonly IUserService _userService;
        public UserManagementController(IApplicationContext<User> applicationContext
            , INomenclatureService nomenclatureService
            , IUserService userService
            ) : base(applicationContext)
        {
            _nomenclatureService = nomenclatureService;
            _userService = userService;
        }



        public async Task<IActionResult> Index()
        {

            var model = new UserManagementFilterModel();
            return View(model);

        }


        public async Task<IActionResult> GetUsers(UserManagementFilterModel model)
        {
            model.PageIndex = model.PageIndex ?? 1;
            model.PageSize = model.PageSize ?? PagedList.DefaultPageSize;

            var filter = Mapper.Map<UserFilterServiceModel>(model);
            var results = await _userService.FilterUser(filter);
            var resultViewModels = Mapper.Map<List<UserModel>>(results.Item1);
            var documentePagedList = PagedList.Create(model.PageIndex.Value, results.Item2, null, resultViewModels);
            return PartialView("_UsersFilteredPartial", documentePagedList);

        }
        public async Task<IActionResult> EditUser(Guid Id)
        {

            var userData = await _userService.GetUserProfile(Id);
            var model = Mapper.Map<UserEditModel>(userData);
            return View(model);

        }

        //[HttpPost]
        //public async Task<IActionResult> EnableDisableUser(long Id, bool enable)
        //{
        //    try
        //    {
        //        await _userService.EnableDisableUser(Id, enable);
        //        var message = new { ok = true, message = "" };
        //        return Ok(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = new { ok = false, message = ex.Message };
        //        return Ok(message);
        //    }

        //}


        //[HttpPost]
        //public async Task<IActionResult> EnableDisable2FaUser(long Id, bool enable)
        //{
        //    try
        //    {
        //        await _userService.EnableDisable2FaUser(Id, enable);
        //        var message = new { ok = true, message = "" };
        //        return Ok(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = new { ok = false, message = ex.Message };
        //        return Ok(message);
        //    }
        //}


        //[HttpPost]
        //public async Task<IActionResult> DisableEnableLockoutEnd(long Id, bool enable)
        //{
        //    try
        //    {
        //        await _userService.DisableEnableLockoutEnd(Id, enable);
        //        var message = new { ok = true, message = "" };
        //        return Ok(message);
        //    }
        //    catch (Exception ex)
        //    {
        //        var message = new { ok = false, message = ex.Message };
        //        return Ok(message);
        //    }
        //}



        public async Task<IActionResult> CreateUser()
        {
            var model = new CreateUserModel();
            return PartialView(model);
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserModel model)
        {
            var request = Mapper.Map<CreateUserIn>(model);
            await _userService.CreateUser(request);
            return RedirectToAction("Index");
        }



    }
}
