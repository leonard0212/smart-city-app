using SmartCity.Domain.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace SmartCity.Database.Stores
{
    public class AppRoleStore : RoleStore<Role, DatabaseContext, Guid>
    {
        public AppRoleStore(DatabaseContext context, IdentityErrorDescriber describer = null)
           : base(context, describer) { }

        protected override IdentityRoleClaim<Guid> CreateRoleClaim(Role role, Claim claim)
        {
            var identityRoleClaim = new IdentityRoleClaim<Guid> { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
            return identityRoleClaim;
        }
    }
}
