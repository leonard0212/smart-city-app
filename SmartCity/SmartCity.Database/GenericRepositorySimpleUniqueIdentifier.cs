using SmartCity.Domain.Models.Users;
using SmartCity.Domain.Models;
using SmartCity.Interfaces.Security;
using Microsoft.AspNetCore.Http;
using SmartCity.Interfaces.Repository;

namespace SmartCity.Database
{
    public class GenericRepositorySimpleUniqueIdentifier<TEntity> : RepositorySimpleUniqueIdentifier<TEntity>, IGenericRepositorySimpleUniqueIdentifier<TEntity> where TEntity : Entity<Guid>
    {
        public GenericRepositorySimpleUniqueIdentifier(
           DatabaseContext db,
           ISecurityContext<User> security,
           IHttpContextAccessor http)
           : base(db, security, http, false) { }
    }
}
