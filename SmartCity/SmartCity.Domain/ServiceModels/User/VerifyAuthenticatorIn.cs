namespace SmartCity.Domain.ServiceModels.User
{
    public class VerifyAuthenticatorIn
    {
        public Guid UserId { get; set; }
        public string VerificationCode { get; set; }


    }
}
