namespace SmartCity.Domain.ServiceModels.User
{
    public class UserFilterResultServiceModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }

        public string AccountName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Claims { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public bool Enabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool IsLockedOut { get; set; }
    }
}
