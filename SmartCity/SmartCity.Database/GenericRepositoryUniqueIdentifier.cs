using SmartCity.Domain.Models.Users;
using SmartCity.Domain.Models;
using SmartCity.Interfaces.Repository;
using SmartCity.Interfaces.Security;
using Microsoft.AspNetCore.Http;

namespace SmartCity.Database
{
    public class GenericRepositoryUniqueIdentifier<TEntity> : RepositoryUniqueIdentifier<TEntity>, IGenericRepositoryUniqueIdentifier<TEntity> where TEntity : EntityComplex<Guid>
    {
        public GenericRepositoryUniqueIdentifier(
       DatabaseContext db,
       ISecurityContext<User> security,
       IHttpContextAccessor http)
        : base(db, security, http)
        { }
    }
}
