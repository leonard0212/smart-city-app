using SmartCity.Domain.Models.Users;
using SmartCity.Domain.Models;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Security;
using Microsoft.AspNetCore.Http;

namespace SmartCity.Database
{
    public class GenericRepositorySimpleLong<TEntity> : RepositorySimpleLong<TEntity>, IGenericRepositorySimpleLong<TEntity> where TEntity : Entity<long>
    {

        public GenericRepositorySimpleLong(
             DatabaseContext db,
             ISecurityContext<User> security,
             IHttpContextAccessor http)
             : base(db, security, http, false) { }



    }
}
