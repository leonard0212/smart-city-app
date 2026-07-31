using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCity.Core.Filters;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Loggers;
using SmartCity.Interfaces.Security;
using System.Net;

namespace SmartCity.Core.Controllers
{


    [LogFormData]
    [Authorize]
    public class SmartCityBaseController : Controller
    {
        protected IApplicationContext<User> _applicationContext { get; }
        protected ISecurityContext<User> SecurityContext => _applicationContext.SecurityContext;
        protected IApplicationLogger ApplicationLogger => _applicationContext.ApplicationLogger;
        protected IWebHostEnvironment HostingEnvironment => _applicationContext.HostingEnvironment;
        protected IActionResult InternalServerError() => StatusCode((int)HttpStatusCode.InternalServerError);
        protected IActionResult UnauthorizedError() => StatusCode((int)HttpStatusCode.Unauthorized);
        protected IMapper Mapper => _applicationContext.Mapper;

        protected const string SessionLastPageKey = "RawDetections.LastPageIndex";
        protected int? GetLastPageIndex()
        => HttpContext?.Session?.GetInt32(SessionLastPageKey);

        protected void SetLastPageIndex(int pageIndex)
            => HttpContext?.Session?.SetInt32(SessionLastPageKey, pageIndex);

        protected void ClearLastPageIndex()
            => HttpContext?.Session?.Remove(SessionLastPageKey);


        protected SmartCityBaseController(IApplicationContext<User> applicationContext)
        {
            _applicationContext = applicationContext;
        }

        //protected IActionResult ActionWrapper(ModelStateDictionary modelState, Func<IActionResult> errorAction, Func<IActionResult> innerActionAsync, string alertsSection = AlertsSections.MainAlerts)
        //{
        //    return ActionWrapperAsync(modelState, errorAction, () => Task.FromResult(innerActionAsync()), alertsSection).Result;
        //}


        //protected async Task<IActionResult> ActionWrapperAsync(ModelStateDictionary modelState, Func<IActionResult> errorAction, Func<Task<IActionResult>> innerActionAsync, string alertsSection = AlertsSections.MainAlerts)
        //{
        //    return await ActionWrapperAsync(modelState, () => Task.FromResult(errorAction()), innerActionAsync, alertsSection);
        //}

        //protected async Task<IActionResult> ActionWrapperAsync(ModelStateDictionary modelState, Func<Task<IActionResult>> errorActionAsync, Func<Task<IActionResult>> innerActionAsync, string alertsSection = AlertsSections.MainAlerts)
        //{
        //    if (!modelState.IsValid)
        //        return (await errorActionAsync()).WithErrors(modelState.AllErrorMessages(), alertsSection);

        //    try
        //    {
        //        var result = await innerActionAsync();
        //        return result;
        //    }
        //    catch (SmartCityException validationException)
        //    {
        //        return (await errorActionAsync()).WithErrors(validationException.Messages, alertsSection);
        //    }
        //    catch (Exception exception)
        //    {
        //        ApplicationLogger.LogError(exception);
        //        return (await errorActionAsync()).WithError("An error has occurred!", alertsSection);
        //    }
        //}


    }
}
