using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.User;
using SmartCity.Domain.ValidatorsServices;
using SmartCity.Interfaces.ValidatorsServices;
using Microsoft.AspNetCore.Identity;

namespace SmartCity.Core.ValidatorsServices
{
    public class UserServiceValidator : ServiceValidatorBase, IUserServiceValidator
    {

        private readonly UserManager<User> _userManager;
      

        public UserServiceValidator(UserManager<User> userManager)
        {
            _userManager = userManager;
           
        }
        public async Task ValidateChangePasswordAsync(ChangePasswordIn input, ValidationResult outValidationResult = null)
        {
            await ValidationActionWrapperAsync(outValidationResult, async validationResult =>
            {

                if (string.IsNullOrEmpty(input?.OldPassword))
                {
                    validationResult.AddError("OldPassword is required");
                    return validationResult;

                }


                var user = input.UserId.HasValue ? await _userManager.FindByIdAsync(input.UserId?.ToString()) : null;
                if (user == null)
                {
                    validationResult.AddError("Username or password is invalid.");
                    return validationResult;
                }

                if (await _userManager.IsLockedOutAsync(user))
                {
                    validationResult.AddError("Account is locked. To unlock please contact an administrator.");
                    return validationResult;
                }

                var checkOldPasswordResult = await _userManager.CheckPasswordAsync(user, input.OldPassword);
                if (!checkOldPasswordResult)
                {
                    await _userManager.AccessFailedAsync(user);
                    validationResult.AddError("Username or password is invalid.");
                    return validationResult;
                }

                if (string.IsNullOrEmpty(input?.Password))
                    validationResult.AddError("Password is required.");

                if (string.IsNullOrEmpty(input?.PasswordConfirmation))
                    validationResult.AddError("Password confirmation is required.");

                if (input?.Password != input?.PasswordConfirmation)
                    validationResult.AddError("Passwords must match.");



                return validationResult;
            });
        }



        //public async Task ValidateGetUserProfileAsync(User user, ValidationResult outValidationResult = null)
        //{
        //    await ValidationActionWrapperAsync(outValidationResult, async validationResult =>
        //    {
        //        var authorized = await _gpAuthorizationService.AuthorizeAccountByEntityAsync(user);
        //        if (!authorized || user == null)
        //            validationResult.AddError(new ErrorMessage($"User not found"));

        //        return validationResult;
        //    });

        //}
    }
}
