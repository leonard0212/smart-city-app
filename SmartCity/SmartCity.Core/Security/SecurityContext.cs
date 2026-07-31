using SmartCity.Database;
using SmartCity.Domain.Models.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace SmartCity.Core.Security
{
    public sealed class SecurityContext : SecurityContextBase
    {
        private readonly IHttpContextAccessor _context;

        public SecurityContext(
           IHttpContextAccessor context,
           DatabaseContext db,
           UserManager<User> userManager,
           RoleManager<Role> roleManager)
           : base(db, userManager, roleManager)
        {
            _context = context;
        }


        public override bool IsAuthenticated
        {
            get
            {
                return (_context.HttpContext?.User.Identity.IsAuthenticated ?? false) &&
                  (
                    _context.HttpContext?.User.Identity.AuthenticationType == "Identity.Application" ||
                    _context.HttpContext?.User.Identity.AuthenticationType == "AuthenticationTypes.Federation"
                  );
            }
        }

        protected override string GetUserName()
        {
            return _context.HttpContext.User.Identity.Name;
        }


    }
}
