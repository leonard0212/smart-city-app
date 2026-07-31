using SmartCity.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace SmartCity.Database
{
    public class UnitOfWork : IUnitOfWork
    {
        public IDbContextTransaction CurrentTransaction { get; private set; }

        private readonly DatabaseContext _applicationDbContext;

        public UnitOfWork(DatabaseContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task ExecuteTransactionalAsync(Func<Task> actionAsync)
        {
            using (CurrentTransaction = _applicationDbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    await actionAsync();
                    CurrentTransaction.Commit();
                }
                catch (Exception)
                {
                    CurrentTransaction.Rollback();
                    throw;
                }
            }
        }
    }
}
