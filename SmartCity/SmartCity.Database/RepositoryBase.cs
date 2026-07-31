using SmartCity.Domain.Models;
using SmartCity.Interfaces.Repository;
using SmartCity.Domain.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SmartCity.Database.Extensions;

namespace SmartCity.Database
{
    public abstract class RepositoryBase<TKey, TEntity> : IRepository<TKey, TEntity>, IDisposable
              where TEntity : Entity<TKey>
    {
        private readonly DatabaseContext _applicationDbContext;

        protected RepositoryBase(DatabaseContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;

        }


        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> where)
        {
            var count = await QueryAll().Where(where).CountAsync();
            return count;
        }

        public async Task<int> CountAllAsync()
        {
            var count = await QueryAll().CountAsync();
            return count;
        }


        public abstract Task<bool> AnyAsync(TKey id);

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> where)
        {
            var any = await QueryAll().AnyAsync(where);
            return any;
        }

        public abstract TEntity Get(TKey id, params Expression<Func<TEntity, object>>[] includes);

        public abstract Task<TEntity> GetAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> @where, params Expression<Func<TEntity, object>>[] includes)
        {
            var result = await QueryAll().IncludeMany(includes).FirstOrDefaultAsync(where);
            return result;
        }

        public async Task<IEnumerable<TEntity>> GetManyAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
        {
            var result = await QueryAll().IncludeMany(includes).Where(where).ToListAsync();
            return result;
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            var entities = await QueryAll().IncludeMany(includes).ToListAsync();
            return entities;
        }

        public abstract Task<TEntity> GetReadUncommittedAsync(TKey id, params Expression<Func<TEntity, object>>[] includes);

        public async Task<IEnumerable<TEntity>> GetManyReadUncommittedAsync(Expression<Func<TEntity, bool>> where, params Expression<Func<TEntity, object>>[] includes)
        {
            var entities = await QueryAll().Where(where).IncludeMany(includes).ToListReadUncommittedAsync(_applicationDbContext);
            return entities;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllReadUncommittedAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            var entities = await QueryAll().IncludeMany(includes).ToListReadUncommittedAsync(_applicationDbContext);
            return entities;
        }

        public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> where,
            params Expression<Func<TEntity, object>>[] includes)
        {
            var result = await GetPagedAsync(pageIndex, pageSize, where, null, includes);
            return result;
        }

        public async Task<IEnumerable<TEntity>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> where,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = QueryAll().IncludeMany(includes).Where(where);

            if (orderBy != null)
                query = orderBy(query);

            var pagedQuery = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            var pagedResult = await pagedQuery.ToListAsync();

            return pagedResult;
        }

        public virtual IQueryable<TEntity> QueryAll()
        {
            return QueryWithNoFilters();
        }

        protected IQueryable<TEntity> QueryWithNoFilters()
        {
            return _applicationDbContext.Set<TEntity>();
        }

        public abstract TKey Save(TEntity entity);

        public abstract Task<TKey> SaveAsync(TEntity entity);

        public abstract void Update(TEntity entity);

        public abstract Task UpdateAsync(TEntity entity);

        public abstract void SaveOrUpdate(TEntity entity);

        public abstract Task SaveOrUpdateAsync(TEntity entity);

        public abstract void Delete(TEntity entity);

        public abstract Task DeleteAsync(TKey key);

        public async Task DeleteAsync(TEntity entity)
        {
            await DeleteAsync(entity.Id);
        }

        public abstract void CommitChanges();

        public abstract Task CommitChangesAsync();

        public DbContext GetDbContext()
        {
            return _applicationDbContext;
        }

        public virtual void Dispose()
        {
            _applicationDbContext.Dispose();
        }

        public abstract IRepository<TKey, TEntity> ToNoFilterGenericRepository();

        public abstract void IgnoreGenericFilters(bool ignore);

    }
}
