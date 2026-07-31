using SmartCity.Domain.Models.Users;
using SmartCity.Domain.Models;
using SmartCity.Domain.Extensions;
using SmartCity.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SmartCity.Interfaces.Security;
using Microsoft.AspNetCore.Http;


namespace SmartCity.Database
{
    public class RepositorySimpleLong<TEntity> : RepositoryBase<long, TEntity>
       where TEntity : Entity<long>
    {
        protected DatabaseContext Db { get; }
        protected ISecurityContext<User> SecurityContext { get; }
        protected IHttpContextAccessor Http { get; }
        protected bool ShouldIgnoreGenericFilters { get; private set; }

        public RepositorySimpleLong(
            DatabaseContext db,
            ISecurityContext<User> security,
            IHttpContextAccessor http)
            : this(db, security, http, false) { }


        protected RepositorySimpleLong(
            DatabaseContext db,
            ISecurityContext<User> security,
            IHttpContextAccessor http,
            bool shouldIgnoreGenericFilters)
            : base(db)
        {
            Db = db;
            SecurityContext = security;
            Http = http;
            ShouldIgnoreGenericFilters = shouldIgnoreGenericFilters;
        }

        public override async Task<bool> AnyAsync(long id)
        {
            var any = await QueryAll().Where(x => x.Id == id).AnyAsync();
            return any;
        }

        public override TEntity Get(long id, params Expression<Func<TEntity, object>>[] includes)
        {
            var entity = QueryAll().IncludeMany(includes).FirstOrDefault(x => x.Id == id);
            return entity;
        }

        public override async Task<TEntity> GetAsync(long id, params Expression<Func<TEntity, object>>[] includes)
        {
            var entity = await QueryAll().IncludeMany(includes).FirstOrDefaultAsync(x => x.Id.Equals(id));
            return entity;
        }

        public override async Task<TEntity> GetReadUncommittedAsync(long id, params Expression<Func<TEntity, object>>[] includes)
        {
            var data = (await GetManyReadUncommittedAsync(x => x.Id.Equals(id), includes)).FirstOrDefault();
            return data;
        }

        public override long Save(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Db.Set<TEntity>().Add(entity).Entity.Id;
        }

        public override Task<long> SaveAsync(TEntity entity)
        {
            var key = Save(entity);
            return Task.FromResult(key);
        }

        public override void Update(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Db.Set<TEntity>().Update(entity);
        }

        public override Task UpdateAsync(TEntity entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        public override void SaveOrUpdate(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id <= 0)
                Save(entity);
            else
                Update(entity);
        }

        public override async Task SaveOrUpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id <= 0)
                await SaveAsync(entity);
            else
                await UpdateAsync(entity);
        }

        public override void Delete(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            Db.Entry(entity).State = EntityState.Deleted;
        }

        public override async Task DeleteAsync(long key)
        {
            var entity = await GetAsync(key);
            if (entity == null)
                throw new ArgumentException(nameof(key), $"Entity not found for key {key}.");

            Db.Entry(entity).State = EntityState.Deleted;
        }

        public override void CommitChanges()
        {
            Db.SaveChanges();
        }

        public override Task CommitChangesAsync()
        {
            CommitChanges();
            return Task.CompletedTask;
        }

        public override IRepository<long, TEntity> ToNoFilterGenericRepository()
        {
            throw new NotImplementedException();
        }

        public override void IgnoreGenericFilters(bool ignore)
        {
            throw new NotImplementedException();
        }
    }
}
