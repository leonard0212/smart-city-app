namespace SmartCity.Domain.ServiceModels.User
{
    public class AuthenticatorDetailsOut
    {
        public string SharedKey { get; set; }

        public string AuthenticatorUri { get; set; }

        public string QrCode { get; set; }
    }
}
