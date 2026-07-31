using SmartCity.Domain.Models.Users;

namespace SmartCity.Domain
{
    public interface IHasAccount
    {
        Guid? AccountId { get; }

        Account Account { get; }
    }
}
