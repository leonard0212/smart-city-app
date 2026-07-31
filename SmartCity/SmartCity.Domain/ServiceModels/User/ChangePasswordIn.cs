namespace SmartCity.Domain.ServiceModels.User
{
    public class ChangePasswordIn
    {
        public Guid? UserId { get; set; }

        public string OldPassword { get; set; }

        public string Password { get; set; }

        public string PasswordConfirmation { get; set; }
    }
}
