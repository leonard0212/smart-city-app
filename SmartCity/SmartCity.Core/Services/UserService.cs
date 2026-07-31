using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.User;
using SmartCity.Interfaces.Services;
using SmartCity.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using SmartCity.Interfaces.ValidatorsServices;
using SmartCity.Core.Exceptions;
using SmartCity.Domain.ValidatorsServices;
using SmartCity.Domain.Utils;
using SmartCity.Core.Utils;

namespace SmartCity.Core.Services
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly UserManager<User> _userManager;

        private readonly IUserServiceValidator _userServiceValidator;
        private readonly UrlEncoder _urlEncoder;
        private readonly SignInManager<User> _signInManager;


        public UserService(IApplicationContext<User> applicationContext
            , UserManager<User> userManager
            , IUserServiceValidator userServiceValidator
            , UrlEncoder urlEncoder
            , SignInManager<User> signInManager
            ) : base(applicationContext)
        {
            _userManager = userManager;
            _userServiceValidator = userServiceValidator;
            _urlEncoder = urlEncoder;
            _signInManager = signInManager;

        }

        public async Task<UserProfileOut> GetUserProfile(Guid userId)
        {
            var user = await _userManager.Users.Include(x => x.Account).Include(x => x.CreatedBy).Where(x => x.Id == userId).FirstOrDefaultAsync();
            //   await _userServiceValidator.ValidateGetUserProfileAsync(user);

            return new UserProfileOut()
            {
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                TwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                Username = user.UserName,
                AccountName = user.Account.CompanyName,
                CreatedAt = user.CreatedAt,
                Enabled = user.Enabled,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd,
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };
        }


        public async Task ChangePasswordAsync(ChangePasswordIn input)
        {
            await _userServiceValidator.ValidateChangePasswordAsync(input);
            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                var user = await _userManager.FindByIdAsync(input.UserId?.ToString());
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                CheckIdentityResult(removePasswordResult);
                var addPasswordResult = await _userManager.AddPasswordAsync(user, input.Password);
                CheckIdentityResult(addPasswordResult);
            });
        }



        public async Task CreateUser(CreateUserIn input)
        {
            var user = new User()
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
                AccountId = input.AccountId ?? SecurityContext.AccountId,
                Email = input.Email,
                UserName = input.Email,
                Enabled = true,
                EmailConfirmed = true,
                CreatedById = SecurityContext.UserId
            };

            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                var result = await _userManager.CreateAsync(user, input.Password);
                CheckIdentityResult(result);


                if (input.Roles != null && input.Roles.Any())
                {
                    var addUserToRoleResult = await _userManager.AddToRolesAsync(user, input.Roles);
                    CheckIdentityResult(addUserToRoleResult);
                }
            });

        }




        public async Task EditUserAsync(EditUserIn input)
        {
            var user = await _userManager.Users.Where(x => x.Id == input.Id).FirstOrDefaultAsync();
            await UnitOfWork.ExecuteTransactionalAsync(async () =>
            {
                user = Mapper.Map(input, user);
                await _userManager.UpdateAsync(user);

                var existingRoles = await _userManager.GetRolesAsync(user);
                var removeFromRoles = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                CheckIdentityResult(removeFromRoles);

                var addToRoles = await _userManager.AddToRolesAsync(user, input.Roles);
                CheckIdentityResult(addToRoles);
            });

        }


        public async Task GenerateUserSessionId(User user)
        {
            user.LastSessionId = Guid.NewGuid().ToString();
            await _userManager.UpdateAsync(user);
        }












        #region TwoFactorAuthentication
        public async Task<AuthenticatorDetailsOut> SetupAuthenticator(Guid userId)
        {

            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(appUser);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(appUser);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(appUser);
            }

            var authenticatorUri = GenerateQrCodeUri(appUser.Email, unformattedKey);

            return new AuthenticatorDetailsOut
            {
                SharedKey = FormatKey(unformattedKey),
                AuthenticatorUri = authenticatorUri,
                QrCode = Convert.ToBase64String(QRCodeUtils.GenerateQR(authenticatorUri))
            };
        }
        public async Task<VerifyAuthenticatorOut> VerifyAuthenticator(VerifyAuthenticatorIn input)
        {
            if (string.IsNullOrEmpty(input.VerificationCode))
                throw new ModelValidationException(new ErrorMessage($"Verification code  is required."));


            var appUser = await _userManager.FindByIdAsync(input.UserId.ToString());

            var verificationCode = input.VerificationCode.Replace(" ", string.Empty).Replace("-", string.Empty);
            var is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(appUser, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2FaTokenValid)
                throw new ModelValidationException(new ErrorMessage($"Verification code {input.VerificationCode} is invalid."));
            await _userManager.SetTwoFactorEnabledAsync(appUser, true);

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(appUser, 4);

            var result = new VerifyAuthenticatorOut()
            {
                RecoveryCodes = recoveryCodes.ToList()
            };
            return result;
        }
        public async Task ResetAuthenticator(long userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());

            await _userManager.SetTwoFactorEnabledAsync(appUser, false);
            await _userManager.ResetAuthenticatorKeyAsync(appUser);

            await _signInManager.RefreshSignInAsync(appUser);
        }

        public async Task Disable2FA(long userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());
            var enabled2Fa = await _userManager.GetTwoFactorEnabledAsync(appUser);

            if (!enabled2Fa)
                throw new ModelValidationException(new ErrorMessage("Cannot disable 2FA as it's not currently enabled."));

            var result = await _userManager.SetTwoFactorEnabledAsync(appUser, false);
        }

        public async Task<GenerateRecoveryCodesOut> GenerateRecoveryCodes(long userId)
        {
            var appUser = await _userManager.FindByIdAsync(userId.ToString());

            var isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(appUser);

            if (!isTwoFactorEnabled)
                throw new ModelValidationException(new ErrorMessage("Cannot generate recovery codes as you do not have 2FA enabled."));

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(appUser, 3);

            var response = new GenerateRecoveryCodesOut();
            response.Codes = new List<string>();
            response.Codes.AddRange(recoveryCodes.ToList());

            return response;
        }





        #endregion


        #region search
        public async Task<(List<UserFilterResultServiceModel>, int)> FilterUser(UserFilterServiceModel filter)
        {

            var where = QueryPredicateBuilder.True<User>();

            if (!string.IsNullOrEmpty(filter.UserName))
                where = where.And(x => x.UserName.ToUpper().Contains(filter.UserName.ToUpper()));

            var totalCount = _userManager.Users.Where(where).Count();
            var queryResult = await _userManager.Users.Include(x => x.Account).Include(x => x.CreatedBy).Where(where).Skip((filter.PageIndex - 1) * filter.PageSize).Take(filter.PageSize).OrderByDescending(x => x.CreatedAt).ToListAsync();


            var result = Mapper.Map<List<UserFilterResultServiceModel>>(queryResult);

            foreach (var user in queryResult)
            {
                var resultUser = result.FirstOrDefault(x => x.Id == user.Id);

                var roles = await _userManager.GetRolesAsync(user);
                var claims = await _userManager.GetClaimsAsync(user);

                resultUser.Roles = roles.ToList();
                resultUser.Claims = claims.Select(x => x.Type).ToList();
            }

            return (result, totalCount);

        }

        #endregion


        #region Helpers
        private string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }
            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }


        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
            return string.Format(
                AuthenticatorUriFormat,
                _urlEncoder.Encode("NewTo"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }



        private void CheckIdentityResult(IdentityResult identityResult)
        {
            if (identityResult.Succeeded)
                return;

            foreach (var error in identityResult.Errors)
                ApplicationLogger.LogWarning($"{error.Code}:{error.Description}");


            var errors = identityResult.GetErrorMessages();
            throw new ValidationException(errors);
        }
        #endregion
    }
}
