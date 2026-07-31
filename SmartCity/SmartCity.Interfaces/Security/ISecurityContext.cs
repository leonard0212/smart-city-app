using SmartCity.Domain.Models.Users;

namespace SmartCity.Interfaces.Security
{
    public interface ISecurityContext<TUser>
    {
        bool IsAuthenticated { get; }

        string UserName { get; }

        Guid? UserId { get; }

        TUser User { get; }

        Guid? AccountId { get; }

        Account Account { get; }

        IEnumerable<System.Security.Claims.Claim> AllClaims { get; }

        IEnumerable<string> UserApplicationRoles { get; }


        bool UserIsInRole(string role);
    }
}
