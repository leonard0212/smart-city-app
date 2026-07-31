namespace SmartCity.Domain.ServiceModels.User
{
    public class CreateUserIn
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }         
        public string Email { get; set; }  
        public Guid? AccountId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public List<string> Roles { get; set; }

    }
}
