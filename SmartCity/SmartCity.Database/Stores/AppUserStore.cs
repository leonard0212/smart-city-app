using SmartCity.Domain.Models.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Database.Stores
{
    public class AppUserStore : UserStore<User, Role, DatabaseContext, Guid>
    {
        public AppUserStore(DatabaseContext context, IdentityErrorDescriber describer = null)
          : base(context, describer) { }

        protected override IdentityUserRole<Guid> CreateUserRole(User user, Role role)
        {
            var applicationUserRole = new UserRole { UserId = user.Id, RoleId = role.Id };
            return applicationUserRole;
        }

        protected override IdentityUserClaim<Guid> CreateUserClaim(User user, Claim claim)
        {
            var identityUserClaim = new IdentityUserClaim<Guid>();
            identityUserClaim.UserId = user.Id;

            var claim1 = claim;
            identityUserClaim.InitializeFromClaim(claim1);

            return identityUserClaim;
        }

        protected override IdentityUserLogin<Guid> CreateUserLogin(User user, UserLoginInfo login)
        {
            var identityUserLogin = new IdentityUserLogin<Guid>
            {
                UserId = user.Id,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider,
                ProviderDisplayName = login.ProviderDisplayName
            };

            return identityUserLogin;
        }

        protected override IdentityUserToken<Guid> CreateUserToken(User user, string loginProvider, string name, string value)
        {
            var identityUserToken = new IdentityUserToken<Guid> { UserId = user.Id, LoginProvider = loginProvider, Name = name, Value = value };
            return identityUserToken;
        }
    }
}
