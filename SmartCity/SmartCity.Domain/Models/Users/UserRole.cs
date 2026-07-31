using Microsoft.AspNetCore.Identity;


namespace SmartCity.Domain.Models.Users
{
    public class UserRole : IdentityUserRole<Guid>
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
