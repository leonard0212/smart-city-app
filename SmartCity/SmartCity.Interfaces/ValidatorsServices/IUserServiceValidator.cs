using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.User;
using SmartCity.Domain.ValidatorsServices;


namespace SmartCity.Interfaces.ValidatorsServices
{
    public interface IUserServiceValidator
    {
        Task ValidateChangePasswordAsync(ChangePasswordIn input, ValidationResult outValidationResult = null);
        //Task ValidateGetUserProfileAsync(User user, ValidationResult outValidationResult = null);
    }
}
