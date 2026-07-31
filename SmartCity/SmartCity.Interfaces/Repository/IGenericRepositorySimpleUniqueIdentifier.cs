namespace SmartCity.Interfaces.Repository
{
    public interface IGenericRepositorySimpleUniqueIdentifier<TEntity> : IRepository<Guid, TEntity> where TEntity : class
    {


    }
}
