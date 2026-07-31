using Microsoft.EntityFrameworkCore.Storage;

namespace SmartCity.Interfaces
{
    public interface IUnitOfWork
    {
        IDbContextTransaction CurrentTransaction { get; }
        Task ExecuteTransactionalAsync(Func<Task> actionAsync);
    }
}
