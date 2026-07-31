using SmartCity.Domain.Models.Users;
using SmartCity.Domain.ServiceModels.User;

namespace SmartCity.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserProfileOut> GetUserProfile(Guid userId);
        Task ChangePasswordAsync(ChangePasswordIn input);

        Task<AuthenticatorDetailsOut> SetupAuthenticator(Guid userId);
        Task<VerifyAuthenticatorOut> VerifyAuthenticator(VerifyAuthenticatorIn input);

        Task<(List<UserFilterResultServiceModel>, int)> FilterUser(UserFilterServiceModel filter);

        Task CreateUser(CreateUserIn input);
        Task EditUserAsync(EditUserIn input);

        Task GenerateUserSessionId(User user);
    }
}
