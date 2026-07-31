using Microsoft.AspNetCore.Identity;

namespace SmartCity.Domain.Models.Users
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {
        }

        public Role(string name)
            : base(name)
        {
        }


    }
}
