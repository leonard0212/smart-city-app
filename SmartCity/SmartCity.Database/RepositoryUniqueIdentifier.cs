using SmartCity.Domain.Models.Users;
using SmartCity.Domain.Models;
using SmartCity.Domain.Extensions;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SmartCity.Database
{
    public class RepositoryUniqueIdentifier<TEntity> : RepositoryBase<Guid, TEntity>
          where TEntity : EntityComplex<Guid>
    {
        private IRepository<Guid, TEntity> _noFilterGenericRepository;
        protected DatabaseContext Db { get; }
        protected ISecurityContext<User> SecurityContext { get; }
        protected IHttpContextAccessor Http { get; }
        protected bool ShouldIgnoreGenericFilters { get; private set; }



        public RepositoryUniqueIdentifier(
       DatabaseContext db,

       ISecurityContext<User> security,
       IHttpContextAccessor http)
       : this(db, security, http, false) { }

        protected RepositoryUniqueIdentifier(
            DatabaseContext db,

            ISecurityContext<User> security,
            IHttpContextAccessor http,
            bool shouldIgnoreGenericFilters
            )

            : base(db)
        {
            Db = db;
            SecurityContext = security;
            Http = http;
            ShouldIgnoreGenericFilters = shouldIgnoreGenericFilters;
        }

        public override async Task<bool> AnyAsync(Guid id)
        {
            var any = await QueryAll().Where(x => x.Id == id).AnyAsync();
            return any;
        }

        public override TEntity Get(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            var entity = QueryAll().IncludeMany(includes).FirstOrDefault(x => x.Id == id);
            return entity;
        }

        public override async Task<TEntity> GetAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            var entity = await QueryAll().IncludeMany(includes).FirstOrDefaultAsync(x => x.Id == id);
            return entity;
        }

        public override async Task<TEntity> GetReadUncommittedAsync(Guid id, params Expression<Func<TEntity, object>>[] includes)
        {
            var data = (await GetManyReadUncommittedAsync(x => x.Id == id, includes)).FirstOrDefault();
            return data;
        }

        public override IQueryable<TEntity> QueryAll()
        {
            return ShouldIgnoreGenericFilters
                ? QueryWithNoFilters()
                : base.QueryAll();
        }

        public override Guid Save(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return Db.Set<TEntity>().Add(entity).Entity.Id;
        }

        public override Task<Guid> SaveAsync(TEntity entity)
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

            if (entity.Id == Guid.Empty)
                Save(entity);
            else
                Update(entity);
        }

        public override async Task SaveOrUpdateAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.Id == Guid.Empty)
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

        public override async Task DeleteAsync(Guid key)
        {
            var entity = await GetAsync(key);
            if (entity == null)
                throw new ArgumentException(nameof(key), $"Entity not found for key {key}.");

            Db.Entry(entity).State = EntityState.Deleted;
        }

        public override void CommitChanges()
        {
            ProcessEntityComplexObjects();
            Db.SaveChanges();
        }

        public override Task CommitChangesAsync()
        {
            CommitChanges();
            return Task.CompletedTask;
        }

        private void ProcessEntityComplexObjects()
        {
            var entries = Db.ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                var _entity = entry.Entity as EntityComplex<Guid>;
                if (_entity != null)
                {
                    if (entry.State == EntityState.Added)
                        _entity.PreInsert(SecurityContext.User, DateTime.Now);
                    if (entry.State == EntityState.Modified)
                        _entity.PreUpdate(SecurityContext.User, DateTime.Now);
                }
            }
        }


        public override IRepository<Guid, TEntity> ToNoFilterGenericRepository()
        {
            return _noFilterGenericRepository ?? (_noFilterGenericRepository = GetNoFilterRepositoryInstance());
        }

        protected virtual RepositoryUniqueIdentifier<TEntity> GetNoFilterRepositoryInstance()
        {
            return new RepositoryUniqueIdentifier<TEntity>(Db, SecurityContext, Http, true);
        }

        public override void IgnoreGenericFilters(bool ignore)
        {
            ShouldIgnoreGenericFilters = ignore;
        }
    }
}
