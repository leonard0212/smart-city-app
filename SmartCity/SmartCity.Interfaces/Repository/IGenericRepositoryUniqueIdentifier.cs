namespace SmartCity.Interfaces.Repository
{
    public interface IGenericRepositoryUniqueIdentifier<TEntity> : IRepository<Guid, TEntity> where TEntity : class
    {
    }
}
