using Microsoft.AspNetCore.Identity;
using SmartCity.Domain.Models.Entities;

namespace SmartCity.Domain.Models.Users
{
    public class User : IdentityUser<Guid>, IHasAccount
    {
        public User()
        {
            CreatedAt = DateTime.Now;
        }

        public Guid? AccountId { get; set; }

        public Account Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public Guid? CreatedById { get; set; }
        public User? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? LastSessionId { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public List<TeamMembership> TeamMemberships { get; set; }



    }
}
