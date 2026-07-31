namespace SmartCity.Domain.Models.Users
{
    public class Account : EntityComplex<Guid>
    {
        public string CompanyName { get; set; }
        public IEnumerable<User> Users { get; set; }
    }
}
