namespace SmartCity.Domain.ServiceModels.User
{
    public class UserProfileOut
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public string FullName { get; set; }
        public string Initials { get; set; }
        public string AccountName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool HasAuthenticator { get; set; }
        public DateTime? CreatedAt { get; set; }

        public List<string> Roles { get; set; }

    }
}
