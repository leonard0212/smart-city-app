using SmartCity.Database;
using SmartCity.Domain.Models.Users;
using SmartCity.Interfaces.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace SmartCity.Core.Security
{
    public abstract class SecurityContextBase : ISecurityContext<User>
    {
        private readonly DatabaseContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        private User _user;
        private IEnumerable<Claim> _allClaims;
        private IEnumerable<string> _userRoles;
        public abstract bool IsAuthenticated { get; }
        protected abstract string GetUserName();
        protected SecurityContextBase(
           DatabaseContext db,
           UserManager<User> userManager,
           RoleManager<Role> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public string UserName
        {
            get { return User?.UserName; }
        }
        public Guid? UserId
        {
            get { return User?.Id; }
        }

        public Guid? AccountId
        {
            get { return Account?.Id; }
        }
        public Account Account
        {
            get { return User?.Account; }
        }

        public User User
        {
            get { return _user ?? (_user = GetUser()); }
        }

        public IEnumerable<Claim> AllClaims
        {
            get { return _allClaims ?? (_allClaims = GetAllClaims().Result); }
        }

        public IEnumerable<string> UserApplicationRoles
        {
            get { return _userRoles ?? (_userRoles = GetUserApplicationRoles()); }
        }

        public bool UserIsInRole(string role)
        {
            var roles = GetUserApplicationRoles();
            return roles.Contains(role);
        }

       

        
        private User GetUser()
        {
            User user = null;

            if (IsAuthenticated)
            {
                user = _db.Set<User>()
                    .Include(u => u.Account)
                    //.Include(u => u.UserExtensionData)
                    .FirstOrDefault(x => x.UserName == GetUserName());
            }
            return user;
        }



        private async Task<IEnumerable<Claim>> GetAllClaims()
        {
            var claims = new List<Claim>();
            if (!IsAuthenticated)
                return claims;

            var userClaims = await _userManager.GetClaimsAsync(User);

            var roleClaims = new List<Claim>();
            var userRoles = await _userManager.GetRolesAsync(User);
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                var cl = await _roleManager.GetClaimsAsync(role);
                roleClaims.Add(new Claim(role.Name, role.Name));
            }

            claims.AddRange(userClaims);
            claims.AddRange(roleClaims);

            return claims;
        }

        private IEnumerable<string> GetUserApplicationRoles()
        {
            var roles = new List<string>();
            if (!IsAuthenticated)
                return roles;

            var rolesNames = _userManager.GetRolesAsync(User).Result;
            roles = rolesNames.Select(roleName => _roleManager.FindByNameAsync(roleName).Result.Name).ToList();
            return roles;
        }



    }
}
