namespace SmartCity.Domain.ServiceModels.User
{
    public class EditUserIn
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool Enabled { get; set; }
        public bool LockoutEnabled { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool HasAuthenticator { get; set; }
        public List<string> Roles { get; set; }
    }
}
