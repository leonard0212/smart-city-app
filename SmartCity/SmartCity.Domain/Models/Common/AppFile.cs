using SmartCity.Domain.Models.Users;

namespace SmartCity.Domain.Models.Common
{
    public class AppFile : Entity<Guid>
    {
        public string? Category { get; set; }
        public string? ContentType { get; set; }
        public string? Extension { get; set; }
        public string? Path { get; set; }
        public string? Name { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string? FullName { get; set; }

    }
}
