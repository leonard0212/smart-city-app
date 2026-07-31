using SmartCity.Core.Controllers;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces;
using SmartCity.Interfaces.Services;
using SmartCity.Web.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmartCity.Web.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : SmartCityBaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAntiforgery _antiforgery;
        private readonly IUserService _userService;

        public AuthenticationController(IApplicationContext<User> applicationContext
          , UserManager<User> userManager
          , SignInManager<User> signInManager
          , IAntiforgery antiforgery
          , IUserService userService) : base(applicationContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _antiforgery = antiforgery;
            _userService = userService;
        }


        [HttpGet]
        public async Task<IActionResult> Login()
        {
            //await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!_antiforgery.IsRequestValidAsync(HttpContext).GetAwaiter().GetResult())
            {
                if (User.Identity.IsAuthenticated)
                    return RedirectToLocal(returnUrl);

                return RedirectToAction("Login");
            }
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    var checkPasswordResult = await _userManager.CheckPasswordAsync(user, model.Password);
                    if (checkPasswordResult)
                    {
                        if (await _userManager.IsLockedOutAsync(user))
                            return View("Lockout");

                    }
                }
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    await _userService.GenerateUserSessionId(user);
                    return RedirectToLocal(returnUrl);
                }
                if (result.IsLockedOut)
                {

                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LogOff()
        {
            if (!_antiforgery.IsRequestValidAsync(HttpContext).GetAwaiter().GetResult())
                return RedirectToAction(nameof(HomeController.Index), "Home");

            await _signInManager.SignOutAsync();



            return RedirectToAction(nameof(HomeController.Index), "Home");
        }





        //  [HttpPost]
        public async Task<IActionResult> ImpersonateUser(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new InvalidOperationException("Nu ati completat utilizatorul.");

            var currentUserName = User.Identities.First().Name;
            var impersonatedUser = await _userManager.FindByNameAsync(username);

            if (impersonatedUser == null)
                throw new InvalidOperationException("Nu exista utilizatorul.");

            var userPrincipal = await _signInManager.CreateUserPrincipalAsync(impersonatedUser);
            userPrincipal.Identities.First().AddClaim(new Claim("OriginalUserName", currentUserName));
            userPrincipal.Identities.First().AddClaim(new Claim("IsImpersonating", "true"));

            await _signInManager.SignOutAsync();

        

            await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme, userPrincipal);





            return RedirectToLocal(null);
        }

        public async Task<IActionResult> StopImpersonation()
        {
            if (!User.HasClaim("IsImpersonating", "true"))
                throw new InvalidOperationException("not impersonating");

            var originalUserName = User.Claims.First(x => x.Type == "OriginalUserName").Value;
            var originalUser = await _userManager.FindByNameAsync(originalUserName);

            await _signInManager.SignOutAsync();

  
            await _signInManager.SignInAsync(originalUser, isPersistent: false);

            return RedirectToLocal(null);
        }
















        #region Helpers
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            else
                return RedirectToAction(nameof(HomeController.Index), "Home");

        }
        #endregion

    }
}
