using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SmartCity.Interfaces.Repository
{
    public interface IRepository<TKey, TEntity> where TEntity : class
    {
        Task<int> CountAsync(Expression<Func<TEntity, bool>> where);

        Task<int> CountAllAsync();

        Task<bool> AnyAsync(TKey id);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where);

        TEntity Get(TKey id, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> GetAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

        Task<TEntity> GetReadUncommittedAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetManyReadUncommittedAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetAllReadUncommittedAsync(params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> where, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includes);

        IQueryable<TEntity> QueryAll();

        TKey Save(TEntity entity);

        Task<TKey> SaveAsync(TEntity entity);

        void Update(TEntity entity);

        Task UpdateAsync(TEntity entity);

        void SaveOrUpdate(TEntity entity);

        Task SaveOrUpdateAsync(TEntity entity);

        void Delete(TEntity entity);

        Task DeleteAsync(TKey key);

        Task DeleteAsync(TEntity entity);

        void CommitChanges();

        Task CommitChangesAsync();

        DbContext GetDbContext();

        IRepository<TKey, TEntity> ToNoFilterGenericRepository();

        void IgnoreGenericFilters(bool ignore);



    }
}
